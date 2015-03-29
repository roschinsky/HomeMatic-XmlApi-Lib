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
        public DateTime LastUpdate { get { return TimeStampToDateTime(LastUpdateTimeStamp); } }

        public override string ToString()
        {
            return String.Format("P >> {0} - Value '{1}' @ {2}", Type, Value, LastUpdate);
        }

        public DateTime TimeStampToDateTime(long timeStamp)
        {
            if(timeStamp != null && timeStamp > 1)
            {
                return (new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).AddSeconds(timeStamp);
            }

            return DateTime.MinValue;
        }
    }
}
