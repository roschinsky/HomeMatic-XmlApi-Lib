using System;
using System.Linq;
using System.Collections.Generic;

namespace TRoschinsky.Lib.HomeMaticXmlApi
{
    public class HMDeviceDataPoint
    {
        public int InternalId { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
        public long LastUpdateTimeStamp { get; set; }
        public DateTime LastUpdate { get { return HMApiWrapper.TimeStampToDateTime(LastUpdateTimeStamp); } }

        public override string ToString()
        {
            return String.Format("P >> {0} - Value '{1}' @ {2}", Type, Value, LastUpdate);
        }
    }
}
