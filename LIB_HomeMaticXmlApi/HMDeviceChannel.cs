using System;
using System.Linq;
using System.Collections.Generic;

namespace TRoschinsky.Lib.HomeMaticXmlApi
{
    public class HMDeviceChannel
    {
        private const string defaultDataTypeName = "STATE";

        public int InternalId { get; set; }
        public string Address { get; set; }
        public string Name { get; set; }
        private string defaultDataType = String.Empty;
        public string DefaultDataType 
        {
            get { return String.IsNullOrEmpty(defaultDataType) ? defaultDataTypeName : defaultDataType; }
            set { defaultDataType = value; } 
        }
        public string PrimaryValue { get { return GetPrimaryDataPoint().Value; } }
        public DateTime PrimaryLastUpdate { get { return GetPrimaryDataPoint().LastUpdate; } }
        private List<HMDeviceDataPoint> dataPoints = new List<HMDeviceDataPoint>();
        public List<HMDeviceDataPoint> DataPoints { get { return dataPoints; } }

        public void AddDataPoint(HMDeviceDataPoint dataPoint)
        {
            dataPoints.Add(dataPoint);
        }

        public override string ToString()
        {
            return String.Format("C:{0} >> {1} - Value '{2}' @ {3}", Address, Name, PrimaryValue, PrimaryLastUpdate);
        }

        private HMDeviceDataPoint GetPrimaryDataPoint()
        {
            try
            {
                return dataPoints.First(p => p.Type == DefaultDataType);
            }
            catch
            {
                return new HMDeviceDataPoint();
            }
        }
    }
}
