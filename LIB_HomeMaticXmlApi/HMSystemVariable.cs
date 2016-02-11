using System;
using System.Collections.Generic;
using System.Globalization;

namespace TRoschinsky.Lib.HomeMaticXmlApi
{
    public class HMSystemVariable : HMBase
    {
        public object Value { get { return TypedValue(); } }
        public string ValueType { get; set; }
        public string ValueString { get; set; }
        public string ValueDescription { get { return GetDescription(); } }
        public string ValueUnit { get; set; }
        public bool IsLogged { get; set; }
        public bool IsVisible { get; set; }
        private Dictionary<object, string> valuesIndex = new Dictionary<object, string>();
        public Dictionary<object, string> ValuesIndex { get { return valuesIndex; } }
        public long LastUpdateTimeStamp { get; set; }
        public DateTime LastUpdate { get { return HMApiWrapper.TimeStampToDateTime(LastUpdateTimeStamp); } }

        private object TypedValue()
        {
            object returnValue;

            // fail over to originating string from XML due to missing value type
            if (String.IsNullOrWhiteSpace(ValueType))
            {
                return ValueString;
            }

            // when typing is active due to existing value type we return null if value string is empty
            if (String.IsNullOrWhiteSpace(ValueString))
            {
                return null;
            }

            // now starting in conversion by given type
            try
            {
                int valueTypeCode = int.Parse(ValueType);

                NumberFormatInfo numFormat = new NumberFormatInfo();
                numFormat.NumberDecimalSeparator = ".";

                switch (valueTypeCode)
                {
                    case 16:
                        returnValue = int.Parse(ValueString);
                        break;

                    case 4:
                    case 6:
                        returnValue = double.Parse(ValueString, numFormat);
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

        private string GetDescription()
        {
            string returnDescription = String.Empty;

            // fail over to empty string
            if (String.IsNullOrWhiteSpace(ValueType) || Value == null)
            {
                return returnDescription;
            }

            // now starting in conversion by given type
            try
            {
                int valueTypeCode = int.Parse(ValueType);

                if (valueTypeCode == 16 || valueTypeCode == 2)
                {
                    returnDescription = valuesIndex[Value];
                }
                else
                {
                    returnDescription = ValueString;
                }

                return returnDescription;
            }
            catch
            {
                return returnDescription;
            }
        }

        public void SetValuesIndex(string trueValue, string falseValue)
        {
            valuesIndex.Clear();
            if (!String.IsNullOrWhiteSpace(trueValue) && !String.IsNullOrWhiteSpace(falseValue))
            {
                valuesIndex.Add(true, trueValue);
                valuesIndex.Add(false, falseValue);
            }
        }

        public void SetValuesIndex(string valueList)
        {
            valuesIndex.Clear();
            if(!String.IsNullOrWhiteSpace(valueList))
            {
                if (valueList.Contains(";"))
                {
                    string[] valueIndexes = valueList.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                    if(valueIndexes != null && valueIndexes.Length > 0)
                    {
                        for (int i = 0; i < valueIndexes.Length; i++)
                        {
                            valuesIndex.Add(i, valueIndexes[i].Trim());
                        }
                    }

                }
                else
                {
                    valuesIndex.Add(0, valueList.Trim());
                }
            }
        }

        public override string ToString()
        {
            return String.Format("VAR: {0} >> Value '{1}' ({2}) @ {3}", Name, ValueDescription, Value, LastUpdate);
        }
    }
}
