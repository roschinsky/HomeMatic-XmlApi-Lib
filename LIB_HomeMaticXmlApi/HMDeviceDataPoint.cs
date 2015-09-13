using System;
using System.Linq;
using System.Collections.Generic;

namespace TRoschinsky.Lib.HomeMaticXmlApi
{
    public class HMDeviceDataPoint
    {
        public int InternalId { get; set; }
        public string Type { get; set; }
        public object Value { get { return Typedvalue(); } }
        public string ValueType { get; set; }
        public string ValueString { get; set; }
        public string ValueUnit { get; set; }
        public long LastUpdateTimeStamp { get; set; }
        public DateTime LastUpdate { get { return HMApiWrapper.TimeStampToDateTime(LastUpdateTimeStamp); } }

        private object Typedvalue()
        {
            object returnValue = null;

            // fail over to originating string from XML due to missing value type
            if (String.IsNullOrWhiteSpace(ValueType))
            {
                return ValueString;
            }

            // when typing is active due to existing value type we return null if value string is empty
            if (String.IsNullOrWhiteSpace(ValueString))
            {
                return returnValue;
            }

            // now starting in conversion by given type
            try
            {
                int valueTypeCode = int.Parse(ValueType);

                switch(valueTypeCode)
                {
                    case 16:
                    case 6:
                        returnValue = int.Parse(ValueString);
                        break;

                    case 4:
                        returnValue = double.Parse(ValueString, System.Globalization.NumberStyles.AllowDecimalPoint);
                        break;

                    case 2:
                        returnValue = bool.Parse(ValueString);
                        break;

                    default:
                        returnValue = ValueString;
                        break;
                }

                return returnValue;
            }
            catch
            {
                return ValueString;
            }
        }

        public override string ToString()
        {
            return String.Format("DP >> {0} - Value '{1}{3}' @ {2}", Type, Value, LastUpdate, (String.IsNullOrEmpty(ValueUnit) ? String.Empty : " " + ValueUnit));
        }
    }
}
