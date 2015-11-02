using System;

namespace TRoschinsky.Lib.HomeMaticXmlApi
{
    public class HMSystemMessage : HMBase
    {
        public long OccuredOnTimeStamp { get; set; }
        public DateTime OccuredOn { get { return HMApiWrapper.TimeStampToDateTime(OccuredOnTimeStamp); } }

        public override string ToString()
        {
            return String.Format("MSG:{0} >> {1} @ {2}", Name, Type, OccuredOn);
        }
    }
}
