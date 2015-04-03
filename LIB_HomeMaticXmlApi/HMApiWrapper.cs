using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Xml;

namespace TRoschinsky.Lib.HomeMaticXmlApi
{
    public class HMApiWrapper
    {
        private string xmlApiDefaultPath = "/addons/xmlapi";

        private string xmlApiMethodDevice = "devicelist";
        private string xmlApiMethodStateAll = "statelist";
        private string xmlApiMethodStateSingle = "state";
        private string xmlApiMethodStateSet = "statechange";
        private string xmlApiMethodVariable = "sysvarlist";

        private Uri HMUrl;

        private List<HMSystemVariable> variables = new List<HMSystemVariable>();
        public List<HMSystemVariable> Variables { get { return variables; } }

        private List<HMSystemMessage> messages = new List<HMSystemMessage>();
        public List<HMSystemMessage> Messages { get { return messages; } }

        private List<HMDevice> devices = new List<HMDevice>();
        public List<HMDevice> Devices { get { return devices; } }


        /// <summary>
        /// Basic constructor
        /// </summary>
        /// <param name="homeMaticUri">Uri to HomeMatic</param>
        public HMApiWrapper(Uri homeMaticUri)
        {
            HMUrl = homeMaticUri;
            Initialize(false, false);
        }

        /// <summary>
        /// Advanced constructor
        /// </summary>
        /// <param name="homeMaticUri">Uri to HomeMatic</param>
        /// <param name="initializeWithStates">Set to true if you want to initialize the wrapper with states; operation takes longer but you are able to access states immediately</param>
        /// <param name="initializeWithVariables">Set to true if you want to initialize the wrapper with HomeMatic system variables</param>
        public HMApiWrapper(Uri homeMaticUri, bool initializeWithStates, bool initializeWithVariables)
        {
            HMUrl = homeMaticUri;
            Initialize(initializeWithStates, initializeWithVariables);
        }

        /// <summary>
        /// Method to initialize the wrapper; gets all available devices and, if needed, states of devices
        /// </summary>
        /// <param name="fullInit">Update all states initially</param>
        /// <param name="variablesInit">Obtain all system variables initially</param>
        private void Initialize(bool fullInit, bool variablesInit)
        {
            devices = GetDevices();
            
            if(fullInit)
            {
                UpdateStates();
            }

            if(variablesInit)
            {
                UpdateVariables();
            }
        }

        /// <summary>
        /// Updates the global list of system variables
        /// </summary>
        public void UpdateVariables()
        {
            // Todo: Implement request of system variables
            throw new NotImplementedException("Implement request of system variables");
        }

        /// <summary>
        /// Updates the global device list including their channels and data point or state data
        /// </summary>
        public void UpdateStates()
        {
            // requesting states list from HomeMatic XmlApi
            XmlDocument xmlStates = GetApiData(xmlApiMethodStateAll);

            // iterating devices
            foreach (XmlElement devElement in xmlStates.DocumentElement.ChildNodes)
            {
                try
                {
                    int devIseId = int.Parse(devElement.GetAttribute("ise_id"));
                    // looking for existing device
                    HMDevice device = devices.First(d => devIseId == d.InternalId);

                    // iterating channels
                    foreach (XmlElement chanElement in devElement.ChildNodes)
                    {
                        try
                        {
                            int chanIseId = int.Parse(chanElement.GetAttribute("ise_id"));
                            // looking for existing channel
                            HMDeviceChannel channel = device.Channels.First(c => chanIseId == c.InternalId);

                            if (channel != null)
                            {
                                // clear all data points to create new
                                channel.DataPoints.Clear();

                                // iterating data points
                                foreach (XmlElement pointElement in chanElement.ChildNodes)
                                {
                                    HMDeviceDataPoint dataPoint = new HMDeviceDataPoint()
                                    {
                                        InternalId = int.Parse(pointElement.GetAttribute("ise_id")),
                                        Type = pointElement.GetAttribute("type"),
                                        LastUpdateTimeStamp = long.Parse(pointElement.GetAttribute("timestamp")),
                                        Value = pointElement.GetAttribute("value")
                                    };

                                    channel.AddDataPoint(dataPoint);
                                }
                            }
                        }
                        catch
                        {
                            // well, maybe there was an channel that is not listed in device list
                            // no problem, we'll just ignore it at this point
                        }
                    }
                }
                catch
                {
                    // well, maybe there was an device that is not listed in device list
                    // no problem, we'll just ignore it at this point
                }
            }
        }

        /// <summary>
        /// Gets all devices including their channels but without any data point or state data
        /// </summary>
        /// <returns>List containing devices with channels</returns>
        private List<HMDevice> GetDevices()
        {
            List<HMDevice> result = new List<HMDevice>();

            // requesting devices list from HomeMatic XmlApi
            XmlDocument xmlDevices = GetApiData(xmlApiMethodDevice);

            // iterating devices
            foreach(XmlElement devElement in xmlDevices.DocumentElement.ChildNodes)
            {
                HMDevice device = new HMDevice()
                {
                    Name = devElement.GetAttribute("name"),
                    Address = devElement.GetAttribute("address"),
                    InternalId = int.Parse(devElement.GetAttribute("ise_id")),
                    DeviceType = devElement.GetAttribute("device_type")
                };

                // iterating channels
                foreach (XmlElement chanElement in devElement.ChildNodes)
                {
                    HMDeviceChannel channel = new HMDeviceChannel()
                    {
                        Name = chanElement.GetAttribute("name"),
                        Address = chanElement.GetAttribute("address"),
                        InternalId = int.Parse(chanElement.GetAttribute("ise_id")),
                    };

                    device.AddChannel(channel);
                }

                result.Add(device);
            }

            return result;
        }

        /// <summary>
        /// Request an XML based API document from HomeMatic Xml Api
        /// </summary>
        /// <param name="apiMethod"></param>
        /// <returns>XML document containing the requested data</returns>
        private XmlDocument GetApiData(string apiMethod)
        {
            XmlDocument result = new XmlDocument();

            if (HMUrl != null)
            {
                WebClient apiClient = new WebClient();
                result.LoadXml(apiClient.DownloadString(String.Format("{0}{1}/{2}.cgi", HMUrl, xmlApiDefaultPath, apiMethod)));

                if(result != null && result.DocumentElement != null)
                {
                return result;    
                }                
            }

            return null;
        }

        /// <summary>
        /// Gets the internal ID for devices or channels by given HomeMatic device or channel address
        /// </summary>
        /// <param name="address">HomeMatic device or channel address</param>
        /// <returns>Internal ID (iseId)</returns>
        private int GetInternalIdByAddress(string address)
        {
            int result = -1;
            bool searchChannels = address.Contains(":");

            HMDevice device = null;
            HMDeviceChannel channel = null;

            device = devices.First(d => address.StartsWith(d.Address));

            if(searchChannels && device != null)
            {
                channel = device.Channels.First(c => c.Address == address);
            }

            if(device != null && !String.IsNullOrEmpty(device.Address))
            {
                result = device.InternalId;

                if(channel != null && !String.IsNullOrEmpty(channel.Address))
                {
                    result = channel.InternalId;
                }
            }

            return result;
        }

        public static DateTime TimeStampToDateTime(long timeStamp)
        {
            if (timeStamp > 1)
            {
                return (new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local)).AddSeconds(timeStamp);
            }

            return DateTime.MinValue;
        }
    }
}
