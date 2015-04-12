using System;

namespace TRoschinsky.Lib.HomeMaticXmlApi
{
    public class HMApiException : Exception
    {
        public string HMApiFault { get; private set; }

        public HMApiException(string message, string hmApiFault) : base(message)
        {
            HMApiFault = hmApiFault;
        }
    }
}
