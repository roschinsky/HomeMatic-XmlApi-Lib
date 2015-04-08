using System;

namespace TRoschinsky.Lib.HomeMaticXmlApi
{
    public class HMSystemMessage
    {
        public int InternalId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public long OccuredOnTimeStamp { get; set; }
        public DateTime OccuredOn { get { return HMApiWrapper.TimeStampToDateTime(OccuredOnTimeStamp); } }

        public override string ToString()
        {
            return String.Format("M:{0} >> {1} @ {2}", Name, Type, OccuredOn);
        }
    }
}
