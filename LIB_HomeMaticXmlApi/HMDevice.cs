using System;
using System.Collections.Generic;

namespace TRoschinsky.Lib.HomeMaticXmlApi
{
    public class HMDevice : HMBase
    {
        public string DeviceType { get; set; }
        private List<HMDeviceChannel> channels = new List<HMDeviceChannel>();
        public List<HMDeviceChannel> Channels { get { return channels; } }

        public void AddChannel(HMDeviceChannel channel)
        {
            channels.Add(channel);
        }

        public override string ToString()
        {
            return String.Format("DEV:{0} >> {1} ({2} Ch.)", Address, Name, channels.Count);
        }
    }
}
