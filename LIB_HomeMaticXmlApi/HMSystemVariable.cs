using System;

namespace TRoschinsky.Lib.HomeMaticXmlApi
{
    public class HMSystemVariable
    {
        public int InternalId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
        public long LastUpdateTimeStamp { get; set; }
        public DateTime LastUpdate { get { return TimeStampToDateTime(LastUpdateTimeStamp); } }

        public override string ToString()
        {
            return String.Format("V:{0} >> {1} @ {2}", Name, Value, LastUpdate);
        }

        public DateTime TimeStampToDateTime(long timeStamp)
        {
            if (timeStamp > 1)
            {
                return (new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local)).AddSeconds(timeStamp);
            }

            return DateTime.MinValue;
        }
    }
}
