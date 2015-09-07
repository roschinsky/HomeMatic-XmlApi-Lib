using System;
using System.Linq;
using System.Collections.Generic;

namespace TRoschinsky.Lib.HomeMaticXmlApi
{
    public class HMDeviceChannel
    {
        private const string default1stDataTypeName = "STATE";
        private const string default2ndDataTypeName = "MOTION";
        private const string default3rdDataTypeName = "ACTUAL_TEMPERATURE";
        private const string default4thDataTypeName = "STICKY_UNREACH";

        public int InternalId { get; set; }
        public string Address { get; set; }
        public string Name { get; set; }
        private string defaultDataType = String.Empty;
        public string DefaultDataType 
        {
            get { return String.IsNullOrEmpty(defaultDataType) ? default1stDataTypeName : defaultDataType; }
            set { defaultDataType = value; } 
        }
        public HMDeviceDataPoint PrimaryDataPoint { get { return GetPrimaryDataPoint(); } }
        public string PrimaryValue { get { return GetPrimaryDataPoint().Value; } }
        public DateTime PrimaryLastUpdate { get { return GetPrimaryDataPoint().LastUpdate; } }
        private Dictionary<string, HMDeviceDataPoint> dataPoints = new Dictionary<string, HMDeviceDataPoint>();
        public Dictionary<string, HMDeviceDataPoint> DataPoints { get { return dataPoints; } }

        public void AddDataPoint(string type, HMDeviceDataPoint dataPoint)
        {
            dataPoints.Add(type, dataPoint);
        }

        public override string ToString()
        {
            return String.Format("C:{0} >> {1} - Value '{2}' @ {3}", Address, Name, PrimaryValue, PrimaryLastUpdate);
        }

        private HMDeviceDataPoint GetPrimaryDataPoint()
        {
            try
            {
                if (dataPoints.Count > 0)
                {
                    if (dataPoints.ContainsKey(default1stDataTypeName))
                    {
                        return dataPoints[default1stDataTypeName];
                    }
                    else if (dataPoints.ContainsKey(default2ndDataTypeName))
                    {
                        return dataPoints[default2ndDataTypeName];
                    }
                    else if (dataPoints.ContainsKey(default3rdDataTypeName))
                    {
                        return dataPoints[default3rdDataTypeName];
                    }
                    else if (dataPoints.ContainsKey(default4thDataTypeName))
                    {
                        return dataPoints[default4thDataTypeName];
                    }
                    else
                    {
                        return dataPoints[dataPoints.Keys.First()];
                    }
                }

                return new HMDeviceDataPoint();
            }
            catch
            {
                return new HMDeviceDataPoint();
            }
        }
    }
}
