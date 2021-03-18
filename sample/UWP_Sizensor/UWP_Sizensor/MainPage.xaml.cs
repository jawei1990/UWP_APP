//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Devices.Enumeration;
using Windows.Devices.Sensors;
using Windows.Foundation;
using Windows.Graphics.Display;
using Windows.Graphics.Imaging;
using Windows.Media;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.System.Display;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace UWP_Sizensor
{
    public sealed partial class MainPage : Page
    {
        // Receive notifications about rotation of the UI and apply any necessary rotation to the preview stream
        private readonly DisplayInformation _displayInformation = DisplayInformation.GetForCurrentView();
        private DisplayOrientations _displayOrientation = DisplayOrientations.Portrait;

        // Rotation metadata to apply to the preview stream (MF_MT_VIDEO_ROTATION)
        // Reference: http://msdn.microsoft.com/en-us/library/windows/apps/xaml/hh868174.aspx
        private static readonly Guid RotationKey = new Guid("C380465D-2271-428C-9B83-ECEA3B4A85C1");

        // Folder in which the captures will be stored (initialized in InitializeCameraAsync)
        private StorageFolder _captureFolder = null;

        // Prevent the screen from sleeping while the camera is running
        private readonly DisplayRequest _displayRequest = new DisplayRequest();

        // For listening to media property changes
        private readonly SystemMediaTransportControls _systemMediaControls = SystemMediaTransportControls.GetForCurrentView();

        // MediaCapture and its state variables
        private MediaCapture _mediaCapture;
        private bool _isInitialized = false;
        private bool _isPreviewing = false;
        private static readonly SemaphoreSlim _mediaCaptureLifeLock = new SemaphoreSlim(1);

        private int BtnCaptureSataus = UtilConst.CAPTURE_STATUS;
        private int BtnCapt_UiStatus = UtilConst.BTN_PREVIEW_STATUS;

        // Information about the camera device
        private bool _mirroringPreview = false;
        private bool _externalCamera = false;

        // Store current frame
        SoftwareBitmap previewFrame;

        private bool _isConnected = false;
        private bool _isLaserOn = false;
        private static BleControl Ble = new BleControl();
        private Accelerometer _accelerometer;


        #region Constructor, lifecycle and navigation

        public MainPage()
        {
            this.InitializeComponent();
            
            // Adjust windows size
            var view = ApplicationView.GetForCurrentView();
            view.TryResizeView(new Size { Width = 800, Height = 600 });
          
            // Cache the UI to have the checkboxes retain their state, as the enabled/disabled state of the
            // GetPreviewFrameButton is reset in code when suspending/navigating (see Start/StopPreviewAsync)
            NavigationCacheMode = NavigationCacheMode.Required;

            // Initialize Ble 
            Ble.BleInit(this);

            // Useful to know when to initialize/clean up the camera
            Application.Current.Suspending += Application_Suspending;
            Application.Current.Resuming += Application_Resuming;
        }

        private async void Application_Suspending(object sender, SuspendingEventArgs e)
        {
            // Handle global application events only if this page is active
            if (Frame.CurrentSourcePageType == typeof(MainPage))
            {
                var deferral = e.SuspendingOperation.GetDeferral();

                await CleanupCameraAsync();

                _displayInformation.OrientationChanged -= DisplayInformation_OrientationChanged;

                deferral.Complete();
            }
        }

        private async void Application_Resuming(object sender, object o)
        {
            // Handle global application events only if this page is active
            if (Frame.CurrentSourcePageType == typeof(MainPage))
            {
                // Populate orientation variables with the current state and register for future changes
                _displayOrientation = _displayInformation.CurrentOrientation;
                _displayInformation.OrientationChanged += DisplayInformation_OrientationChanged;

                await InitializeCameraAsync();
            }
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            // Populate orientation variables with the current state and register for future changes
            _displayOrientation = _displayInformation.CurrentOrientation;
            _displayInformation.OrientationChanged += DisplayInformation_OrientationChanged;

            await InitializeCameraAsync();
            
            // Start searching Ble device
            Ble.BleSearching();

            _accelerometer = Accelerometer.GetDefault(AccelerometerReadingType);
            if (_accelerometer != null)
            {
                Debug.WriteLine(AccelerometerReadingType + " accelerometer ready");
                
                // Enable acclerometer
                ScenarioEnable();
            }
            else
            {
                Debug.WriteLine(AccelerometerReadingType + " accelerometer not found");
                AlertDialogSingleForBtn("Hardware Not Sup","Your device not support accelerometer.","OK");
            }
        }

        protected override async void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            // Handling of this event is included for completenes, as it will only fire when navigating between pages and this sample only includes one page

            await CleanupCameraAsync();

            _displayInformation.OrientationChanged -= DisplayInformation_OrientationChanged;
            ScenarioDisable();
        }

        #endregion Constructor, lifecycle and navigation


        #region Event handlers

        /// <summary>
        /// In the event of the app being minimized this method handles media property change events. If the app receives a mute
        /// notification, it is no longer in the foregroud.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private async void SystemMediaControls_PropertyChanged(SystemMediaTransportControls sender, SystemMediaTransportControlsPropertyChangedEventArgs args)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                // Only handle this event if this page is currently being displayed
                if (args.Property == SystemMediaTransportControlsProperty.SoundLevel && Frame.CurrentSourcePageType == typeof(MainPage))
                {
                    // Check to see if the app is being muted. If so, it is being minimized.
                    // Otherwise if it is not initialized, it is being brought into focus.
                    if (sender.SoundLevel == SoundLevel.Muted)
                    {
                        await CleanupCameraAsync();
                    }
                    else if (!_isInitialized)
                    {
                        await InitializeCameraAsync();
                    }
                }
            });
        }

        /// <summary>
        /// This event will fire when the page is rotated
        /// </summary>
        /// <param name="sender">The event source.</param>
        /// <param name="args">The event data.</param>
        private async void DisplayInformation_OrientationChanged(DisplayInformation sender, object args)
        {
            _displayOrientation = sender.CurrentOrientation;

            if (_isPreviewing)
            {
                await SetPreviewRotationAsync();
            }
        }

        private void ShowView(RelativePanel layout,Image view,String StrUri)
        {
            FileView.Visibility = Visibility.Collapsed;
            BtnFileBG.Source = new BitmapImage() { UriSource = new Uri("ms-appx:///Assets/png_folder_un.png", UriKind.Absolute) };

            if (view != null)
            {
                view.Source = new BitmapImage() { UriSource = new Uri(StrUri, UriKind.Absolute) };
            }

            if (layout != null)
            {
                layout.Visibility = Visibility.Visible;
            }
        }

        private async void MediaCapture_Failed(MediaCapture sender, MediaCaptureFailedEventArgs errorEventArgs)
        {
            Debug.WriteLine("MediaCapture_Failed: (0x{0:X}) {1}", errorEventArgs.Code, errorEventArgs.Message);

            await CleanupCameraAsync();

            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => BtnCapture.IsEnabled = _isPreviewing);
        }

        #endregion Event handlers


        #region MediaCapture methods

        /// <summary>
        /// Initializes the MediaCapture, registers events, gets camera device information for mirroring and rotating, and starts preview
        /// </summary>
        /// <returns></returns>
        private async Task InitializeCameraAsync()
        {
            Debug.WriteLine("InitializeCameraAsync");

            await _mediaCaptureLifeLock.WaitAsync();

            if (_mediaCapture == null)
            {
                // Attempt to get the back camera if one is available, but use any camera device if not
                var cameraDevice = await FindCameraDeviceByPanelAsync(Windows.Devices.Enumeration.Panel.Back);

                if (cameraDevice == null)
                {
                    Debug.WriteLine("No camera device found!");
                    AlertDialogSingleForBtn("Camera Error", "No camera device found!", "OK");
                    _mediaCaptureLifeLock.Release();
                    return;
                }

                // Create MediaCapture and its settings
                _mediaCapture = new MediaCapture();

                // Register for a notification when something goes wrong
                _mediaCapture.Failed += MediaCapture_Failed;

                var settings = new MediaCaptureInitializationSettings { VideoDeviceId = cameraDevice.Id };

                // Initialize MediaCapture
                try
                {
                    await _mediaCapture.InitializeAsync(settings);
                    _isInitialized = true;
                }
                catch (UnauthorizedAccessException)
                {
                    Debug.WriteLine("The app was denied access to the camera");
                }
                finally
                {
                    _mediaCaptureLifeLock.Release();
                }

                // If initialization succeeded, start the preview
                if (_isInitialized)
                {
                    // Figure out where the camera is located
                    if (cameraDevice.EnclosureLocation == null || cameraDevice.EnclosureLocation.Panel == Windows.Devices.Enumeration.Panel.Unknown)
                    {
                        // No information on the location of the camera, assume it's an external camera, not integrated on the device
                        _externalCamera = true;
                    }
                    else
                    {
                        // Camera is fixed on the device
                        _externalCamera = false;

                        // Only mirror the preview if the camera is on the front panel
                        _mirroringPreview = (cameraDevice.EnclosureLocation.Panel == Windows.Devices.Enumeration.Panel.Front);
                    }


                    await StartPreviewAsync();

                    var picturesLibrary = await StorageLibrary.GetLibraryAsync(KnownLibraryId.Pictures);
                    // Fall back to the local app storage if the Pictures Library is not available
                    _captureFolder = picturesLibrary.SaveFolder ?? ApplicationData.Current.LocalFolder;
                }
            }
            else
            {
                _mediaCaptureLifeLock.Release();
            }
        }

        /// <summary>
        /// Starts the preview and adjusts it for for rotation and mirroring after making a request to keep the screen on and unlocks the UI
        /// </summary>
        /// <returns></returns>
        private async Task StartPreviewAsync()
        {
            Debug.WriteLine("StartPreviewAsync");

            // Prevent the device from sleeping while the preview is running
            _displayRequest.RequestActive();

            // Register to listen for media property changes
            _systemMediaControls.PropertyChanged += SystemMediaControls_PropertyChanged;

            // Set the preview source in the UI and mirror it if necessary
            PreviewControl.Source = _mediaCapture;
            PreviewControl.FlowDirection = _mirroringPreview ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;

            // Start the preview
            await _mediaCapture.StartPreviewAsync();
            _isPreviewing = true;

            // Initialize the preview to the current orientation
            if (_isPreviewing)
            {
                await SetPreviewRotationAsync();
            }
        }

        /// <summary>
        /// Gets the current orientation of the UI in relation to the device and applies a corrective rotation to the preview
        /// </summary>
        private async Task SetPreviewRotationAsync()
        {
            // Only need to update the orientation if the camera is mounted on the device
            if (_externalCamera) return;

            // Calculate which way and how far to rotate the preview
            int rotationDegrees = ConvertDisplayOrientationToDegrees(_displayOrientation);

            // The rotation direction needs to be inverted if the preview is being mirrored
            if (_mirroringPreview)
            {
                rotationDegrees = (360 - rotationDegrees) % 360;
            }

            // Add rotation metadata to the preview stream to make sure the aspect ratio / dimensions match when rendering and getting preview frames
            var props = _mediaCapture.VideoDeviceController.GetMediaStreamProperties(MediaStreamType.VideoPreview);
            props.Properties.Add(RotationKey, rotationDegrees);
            await _mediaCapture.SetEncodingPropertiesAsync(MediaStreamType.VideoPreview, props, null);
        }

        /// <summary>
        /// Stops the preview and deactivates a display request, to allow the screen to go into power saving modes, and locks the UI
        /// </summary>
        /// <returns></returns>
        private async Task StopPreviewAsync()
        {
            _isPreviewing = false;
            await _mediaCapture.StopPreviewAsync();

            // Use the dispatcher because this method is sometimes called from non-UI threads
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                PreviewControl.Source = null;

                // Allow the device to sleep now that the preview is stopped
                _displayRequest.RequestRelease();

                //BtnCapture.IsEnabled = _isPreviewing;
            });
        }

        /// <summary>
        /// Gets the current preview frame as a SoftwareBitmap, displays its properties in a TextBlock, and can optionally display the image
        /// in the UI and/or save it to disk as a jpg
        /// </summary>
        /// <returns></returns>
        private async Task GetPreviewFrameAsSoftwareBitmapAsync()
        {
            // Get information about the preview
            var previewProperties = _mediaCapture.VideoDeviceController.GetMediaStreamProperties(MediaStreamType.VideoPreview) as VideoEncodingProperties;

            // Create the video frame to request a SoftwareBitmap preview frame
            var videoFrame = new VideoFrame(BitmapPixelFormat.Bgra8, (int)previewProperties.Width, (int)previewProperties.Height);

            // Capture the preview frame
            using (var currentFrame = await _mediaCapture.GetPreviewFrameAsync(videoFrame))
            {
                // Collect the resulting frame
                previewFrame = currentFrame.SoftwareBitmap;

                // Show the frame (as is, no rotation is being applied)
                {
                    // Create a SoftwareBitmapSource to display the SoftwareBitmap to the user
                    var sbSource = new SoftwareBitmapSource();
                    await sbSource.SetBitmapAsync(previewFrame);

                // Display it in the Image control
                    PreviewFrameImage.Source = sbSource;

                    // Stop Preview mode and change UI 
                    ChangeBtnCaptureIcon(UtilConst.BTN_PREVIEW_STATUS);
                    //await StopPreviewAsync();
                    BtnCaptureSataus = UtilConst.EDIT_STATUS;

                }
            }
        }

        private void ChangeBtnCaptureIcon(int UI_status)
        {
            switch (UI_status)
            {
                case UtilConst.BTN_PREVIEW_STATUS:
                    BtnCaptureBG.Source = new BitmapImage() { UriSource = new Uri("ms-appx:///assets/png_camera_preview_un.png", UriKind.Absolute) };
                    break;
                case UtilConst.BTN_AUTO_STATUS:
                    break;
                case UtilConst.BTN_CAPTURE_STATUS: 
                    BtnCaptureBG.Source = new BitmapImage() { UriSource = new Uri("ms-appx:///assets/png_camera_un.png", UriKind.Absolute) };
                    break;
                case UtilConst.BTN_OPEN_LASER_STATUS:
                    break;
                case UtilConst.BTN_CALIBRATION_HINT:
                    break;
                case UtilConst.BTN_LASER_OFF:
                    break;
            }           
            BtnCapt_UiStatus = UI_status;
        }

        /// <summary>
        /// Gets the current preview frame as a Direct3DSurface and displays its properties in a TextBlock
        /// </summary>
        /// <returns></returns>
        private async Task GetPreviewFrameAsD3DSurfaceAsync()
        {
            // Capture the preview frame as a D3D surface
            using (var currentFrame = await _mediaCapture.GetPreviewFrameAsync())
            {
                // Check that the Direct3DSurface isn't null. It's possible that the device does not support getting the frame
                // as a D3D surface
                if (currentFrame.Direct3DSurface != null)
                {
                    // Collect the resulting frame
                    var surface = currentFrame.Direct3DSurface;
                }
                else // Fall back to software bitmap
                {
                    // Collect the resulting frame
                    SoftwareBitmap previewFrame = currentFrame.SoftwareBitmap;
                }

                // Clear the image
                PreviewFrameImage.Source = null;
            }
        }

        /// <summary>
        /// Cleans up the camera resources (after stopping the preview if necessary) and unregisters from MediaCapture events
        /// </summary>
        /// <returns></returns>
        private async Task CleanupCameraAsync()
        {
            await _mediaCaptureLifeLock.WaitAsync();

            try
            {
                if (_isInitialized)
                {
                    if (_isPreviewing)
                    {
                        // The call to stop the preview is included here for completeness, but can be
                        // safely removed if a call to MediaCapture.Dispose() is being made later,
                        // as the preview will be automatically stopped at that point
                        await StopPreviewAsync();
                    }

                    _isInitialized = false;
                }

                if (_mediaCapture != null)
                {
                    _mediaCapture.Failed -= MediaCapture_Failed;
                    _mediaCapture.Dispose();
                    _mediaCapture = null;
                }

            }
            finally
            {
                _mediaCaptureLifeLock.Release();
            }
        }

        #endregion MediaCapture methods


        #region Helper functions

        /// <summary>
        /// Queries the available video capture devices to try and find one mounted on the desired panel
        /// </summary>
        /// <param name="desiredPanel">The panel on the device that the desired camera is mounted on</param>
        /// <returns>A DeviceInformation instance with a reference to the camera mounted on the desired panel if available,
        ///          any other camera if not, or null if no camera is available.</returns>
        private static async Task<DeviceInformation> FindCameraDeviceByPanelAsync(Windows.Devices.Enumeration.Panel desiredPanel)
        {
            // Get available devices for capturing pictures
            var allVideoDevices = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);

            // Get the desired camera by panel
            DeviceInformation desiredDevice = allVideoDevices.FirstOrDefault(x => x.EnclosureLocation != null && x.EnclosureLocation.Panel == desiredPanel);

            // If there is no device mounted on the desired panel, return the first device found
            return desiredDevice ?? allVideoDevices.FirstOrDefault();
        }

        /// <summary>
        /// Converts the given orientation of the app on the screen to the corresponding rotation in degrees
        /// </summary>
        /// <param name="orientation">The orientation of the app on the screen</param>
        /// <returns>An orientation in degrees</returns>
        private static int ConvertDisplayOrientationToDegrees(DisplayOrientations orientation)
        {
            switch (orientation)
            {
                case DisplayOrientations.Portrait:
                    return 90;
                case DisplayOrientations.LandscapeFlipped:
                    return 180;
                case DisplayOrientations.PortraitFlipped:
                    return 270;
                case DisplayOrientations.Landscape:
                default:
                    return 0;
            }
        }

        /// <summary>
        /// Saves a SoftwareBitmap to the specified StorageFile
        /// </summary>
        /// <param name="bitmap">SoftwareBitmap to save</param>
        /// <param name="file">Target StorageFile to save to</param>
        /// <returns></returns>
        private static async Task SaveSoftwareBitmapAsync(SoftwareBitmap bitmap, StorageFile file)
        {
            using (var outputStream = await file.OpenAsync(FileAccessMode.ReadWrite))
            {
                var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, outputStream);

                // Grab the data from the SoftwareBitmap
                encoder.SetSoftwareBitmap(bitmap);
                await encoder.FlushAsync();
            }
        }

        /// <summary>
        /// Applies a basic effect to a Bgra8 SoftwareBitmap in-place
        /// </summary>
        /// <param name="bitmap">SoftwareBitmap that will receive the effect</param>
        //private unsafe void ApplyGreenFilter(SoftwareBitmap bitmap)
        //{
        //    // Effect is hard-coded to operate on BGRA8 format only
        //    if (bitmap.BitmapPixelFormat == BitmapPixelFormat.Bgra8)
        //    {
        //        // In BGRA8 format, each pixel is defined by 4 bytes
        //        const int BYTES_PER_PIXEL = 4;

        //        using (var buffer = bitmap.LockBuffer(BitmapBufferAccessMode.ReadWrite))
        //        using (var reference = buffer.CreateReference())
        //        {
        //            if (reference is IMemoryBufferByteAccess)
        //            {
        //                // Get a pointer to the pixel buffer
        //                byte* data;
        //                uint capacity;
        //                ((IMemoryBufferByteAccess)reference).GetBuffer(out data, out capacity);

        //                // Get information about the BitmapBuffer
        //                var desc = buffer.GetPlaneDescription(0);

        //                // Iterate over all pixels
        //                for (uint row = 0; row < desc.Height; row++)
        //                {
        //                    for (uint col = 0; col < desc.Width; col++)
        //                    {
        //                        // Index of the current pixel in the buffer (defined by the next 4 bytes, BGRA8)
        //                        var currPixel = desc.StartIndex + desc.Stride * row + BYTES_PER_PIXEL * col;

        //                        // Read the current pixel information into b,g,r channels (leave out alpha channel)
        //                        var b = data[currPixel + 0]; // Blue
        //                        var g = data[currPixel + 1]; // Green
        //                        var r = data[currPixel + 2]; // Red

        //                        // Boost the green channel, leave the other two untouched
        //                        data[currPixel + 0] = b;
        //                        data[currPixel + 1] = (byte)Math.Min(g + 80, 255);
        //                        data[currPixel + 2] = r;
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}

        #endregion Helper functions 

        #region accelerometer
        public AccelerometerReadingType AccelerometerReadingType { get; set; } = AccelerometerReadingType.Standard;

        public static void SetReadingText(TextBlock textBlock, AccelerometerReading reading)
        {
            String strAcce = string.Format("X: {0,5:0.00}, Y: {1,5:0.00}, Z: {2,5:0.00}",
                reading.AccelerationX, reading.AccelerationY, reading.AccelerationZ);
            Debug.WriteLine("Accelerometer:" + strAcce);
        }
        /// This is the event handler for ReadingChanged events.
        async private void ReadingChanged(object sender, AccelerometerReadingChangedEventArgs e)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                MainPage.SetReadingText(disStr, e.Reading);
            });
        }

        private void ScenarioEnable()
        {
            // Select a report interval that is both suitable for the purposes of the app and supported by the sensor.
            _accelerometer.ReportInterval = Math.Max(_accelerometer.MinimumReportInterval, 16);

            Window.Current.VisibilityChanged += VisibilityChanged;
            _accelerometer.ReadingChanged += ReadingChanged;
        }

        private void ScenarioDisable()
        {
            Window.Current.VisibilityChanged -= VisibilityChanged;
            _accelerometer.ReadingChanged -= ReadingChanged;

            // Restore the default report interval to release resources while the sensor is not in use
            _accelerometer.ReportInterval = 0;
        }

        private void VisibilityChanged(object sender, VisibilityChangedEventArgs e)
        {
            if (e.Visible)
            {
                // Re-enable sensor input (no need to restore the desired reportInterval... it is restored for us upon app resume)
                _accelerometer.ReadingChanged += ReadingChanged;
            }
            else
            {
                // Disable sensor input (no need to restore the default reportInterval... resources will be released upon app suspension)
                _accelerometer.ReadingChanged -= ReadingChanged;
            }
        }
        #endregion accelerometer

        #region UI Display 
        // Disconnected: 0, Connecting :1, Connected:2
        public void ChangeBleIcon(int status)
        {
            switch (status)
            {
                case BleConsts.STATE_DISCONNECTED:
                    _isConnected = false;
                    BleLink.Source = new BitmapImage() { UriSource = new Uri("ms-appx:///assets/png_bt_red.png", UriKind.Absolute) };
                    break;
                case BleConsts.STATE_CONNECTING:
                    BleLink.Source = new BitmapImage() { UriSource = new Uri("ms-appx:///assets/png_bt_white.png", UriKind.Absolute) };
                    break;
                case BleConsts.STATE_CONNECTED:   
                    _isConnected = true;
                    BleLink.Source = new BitmapImage() { UriSource = new Uri("ms-appx:///assets/png_bt_blue.png", UriKind.Absolute) };
                    break;
            }
        }

        public async void ShowDistance(string str)
        {
            try
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () => disStr.Text = str);
            }
            catch (Exception e)
            {
                Debug.WriteLine("ShowDistance exception:" + e.ToString());
            }

        }

        #endregion UI Display 

        private async void AlertDialogSingleForBtn(String sTitle, String sContent,String btnStr)
        {
            ContentDialog Dialog = new ContentDialog
            {
                Title = sTitle,
                Content = sContent,
                CloseButtonText = btnStr,
            };

            ContentDialogResult result = await Dialog.ShowAsync();
        }



        #region BtnOnClick
        private void BtnFileOnClick(object sender, RoutedEventArgs e)
        {
            if (FileView.Visibility == Visibility.Collapsed)
                ShowView(FileView, BtnFileBG, "ms-appx:///Assets/png_folder_open_sel.png");
            else
                ShowView(null, BtnFileBG, "ms-appx:///Assets/png_folder_un.png");
        }

        private async void BtnCaptureOnClick(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("BLE:" + (_isConnected?"Connected":"Disconnected"));
            if (_isConnected)
            {
                ShowView(null, null, "");
                // If preview is not running, no preview frames can be acquired
                if (!_isPreviewing) return;

                Debug.WriteLine("BtnCaptureSataus::" + BtnCaptureSataus);
                switch (BtnCaptureSataus)
                {
                    case UtilConst.CAPTURE_STATUS:
                        await GetPreviewFrameAsSoftwareBitmapAsync();
                        break;
                    case UtilConst.EDIT_STATUS:
                        //await StartPreviewAsync();

                        // Clear the image
                        PreviewFrameImage.Source = null;
                        ChangeBtnCaptureIcon(UtilConst.BTN_CAPTURE_STATUS);
                        BtnCaptureSataus = UtilConst.CAPTURE_STATUS;
                        break;
                }
            }
            else 
            {
                AlertDialogSingleForBtn("BLE Not Connected", "BLE not connected,please trun on the laser device.", "OK");
                Ble.BleSearching();
            }
        }

        private async void BtnOpenOnClick(object sender, RoutedEventArgs e)
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");

            Windows.Storage.StorageFile file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                Debug.WriteLine("Open photo:" + file.Name);

                // Ensure the stream is disposed once the image is loaded
                using (IRandomAccessStream fileStream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read))
                {   
                    var decoder = await BitmapDecoder.CreateAsync(fileStream);
                    var softBitmap = await decoder.GetSoftwareBitmapAsync(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied);

                    // Use SoftwareBitmapSource to ImageSource
                    var source = new SoftwareBitmapSource();
                    await source.SetBitmapAsync(softBitmap);
                    PreviewFrameImage.Source = source;

                    // Stop Preview mode and change UI 
                    ChangeBtnCaptureIcon(UtilConst.BTN_PREVIEW_STATUS);
                    BtnCaptureSataus = UtilConst.EDIT_STATUS;
                }
            }
            else 
            {
                AlertDialogSingleForBtn("Open Image Warning", "Could not open the image", "OK");
            }
        }

        private async void BtnSaveOnClick(object sender, RoutedEventArgs e)
        {
            if (BtnCaptureSataus == UtilConst.EDIT_STATUS)
                await SaveBitmapAsync();
            else
                AlertDialogSingleForBtn("Save Image Warning", "Could not save photo without capture.", "OK");
        }
        #endregion BtnOnClick

        private async Task SaveBitmapAsync()
        {
            try
            {
                DateTime localDate = DateTime.Now;
                String file_name = "Sizesor_" + localDate.ToString("MMdd_HHmmss") + ".jpg";
                Debug.WriteLine("Pic name:" + file_name);
                var var_file = await _captureFolder.CreateFileAsync(file_name, CreationCollisionOption.GenerateUniqueName);
                Debug.WriteLine("Photo taken! Saving to " + var_file.Path);
                await SaveSoftwareBitmapAsync(previewFrame, var_file);
            }
            catch (Exception e)
            {
                // File I/O errors are reported as exceptions
                Debug.WriteLine("Exception when taking a photo: " + e.ToString());
            }
        }
    }
}