using System;

namespace TRoschinsky.Lib.HomeMaticXmlApi
{
    public class HMSystemMessage
    {
        public int InternalId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
        public long LastUpdateTimeStamp { get; set; }
        public DateTime LastUpdate { get { return HMApiWrapper.TimeStampToDateTime(LastUpdateTimeStamp); } }

        public override string ToString()
        {
            return String.Format("V:{0} >> {1} @ {2}", Name, Value, LastUpdate);
        }
    }
}
