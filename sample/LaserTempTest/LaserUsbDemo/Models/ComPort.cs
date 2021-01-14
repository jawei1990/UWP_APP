using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaserUsbDemo.Models
{
    public class ComPort
    {
        public string name;
        public string description;

        public ComPort(string name, string description)
        {
            this.name = name;
            this.description = description;
        }
    }
}