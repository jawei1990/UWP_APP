using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace TestBle
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        bool isConnected = false;
        bool isLaserOn = false;
        private static BleControl Ble = new BleControl(); 
        public MainPage()
        {
            this.InitializeComponent();
            Ble.BleInit(this);
           
        }

        // Disconnected: 0, Connecting :1, Connected:2
        public void ChangeBleIcon(int status)
        {
            switch (status)
            {
                case BleConsts.STATE_DISCONNECTED:
                    BleLink.Source = new BitmapImage() { UriSource = new Uri("ms-appx:///assets/png_bt_red.png", UriKind.Absolute) };
                    break;
                case BleConsts.STATE_CONNECTING:
                    BleLink.Source = new BitmapImage() { UriSource = new Uri("ms-appx:///assets/png_bt_white.png", UriKind.Absolute) };
                    break;
                case BleConsts.STATE_CONNECTED:
                    BleLink.Source = new BitmapImage() { UriSource = new Uri("ms-appx:///assets/png_bt_blue.png", UriKind.Absolute) };
                    break;
            }
        }

        private void BntSearcingOnClick(object sender, RoutedEventArgs e)
        {
            if (!isConnected)
                Ble.BleSearching();
            else
                Ble.BleDisConnecting();
        }

        private async void BtnLaserCtrOnClick(object sender, RoutedEventArgs e)
        {
            if (isConnected)
            {
                if (!isLaserOn)
                    await Ble.BleSend("O");
                else
                    await Ble.BleSend("C");
            }

            ContentDialog contentDialog = new ContentDialog()
            {
                Title = "BLE Not Connected",
                Content = "BLE is not connected,please re-connected.",
                CloseButtonText = "Got it."
            };

        }

        private async void BtnGetDisOnClick(object sender, RoutedEventArgs e)
        {
            if (isConnected)
            {
                await Ble.BleSend("D");
            }

            ContentDialog contentDialog = new ContentDialog()
            {
                Title = "BLE Not Connected",
                Content = "BLE is not connected,please re-connected.",
                CloseButtonText = "Got it."
            };
        }
    }
}
