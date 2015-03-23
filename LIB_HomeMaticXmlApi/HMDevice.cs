using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRoschinsky.Lib.HomeMaticXmlApi
{
    public class HMDevice
    {
        public int InternalId { get; set; }
        public string Address { get; set; }
        public string Name { get; set; }
        public string DeviceType { get; set; }
        private List<HMDeviceChannel> channels = new List<HMDeviceChannel>();
        public List<HMDeviceChannel> Channels { get { return channels; } }

        public void AddChannel(HMDeviceChannel channel)
        {
            channels.Add(channel);
        }

        public override string ToString()
        {
            return String.Format("D:{0} >> {1} ({2} Ch.)", Address, Name, channels.Count);
        }
    }
}
