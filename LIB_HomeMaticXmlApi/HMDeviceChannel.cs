using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRoschinsky.Lib.HomeMaticXmlApi
{
    public class HMDeviceChannel : HMDevice
    {
        public string Value { get; set; }
        public DateTime LastUpdate { get; set; }
    }
}
