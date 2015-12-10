using System;

namespace TRoschinsky.Lib.HomeMaticXmlApi
{
    public class HMSystemMessage : HMBase
    {
        public long OccurredOnTimeStamp { get; set; }
        public DateTime OccurredOn { get { return HMApiWrapper.TimeStampToDateTime(OccurredOnTimeStamp); } }

        public override string ToString()
        {
            return String.Format("MSG:{0} >> {1} @ {2}", Name, Type, OccurredOn);
        }
    }
}
