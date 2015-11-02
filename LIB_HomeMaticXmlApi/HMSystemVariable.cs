using System;

namespace TRoschinsky.Lib.HomeMaticXmlApi
{
    public class HMSystemVariable : HMBase
    {
        public string Value { get; set; }
        public long LastUpdateTimeStamp { get; set; }
        public DateTime LastUpdate { get { return HMApiWrapper.TimeStampToDateTime(LastUpdateTimeStamp); } }

        public override string ToString()
        {
            return String.Format("VAR:{0} >> {1} @ {2}", Name, Value, LastUpdate);
        }
    }
}
