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
        private string xmlApiMethodMessage = "systemNotification";

        private Uri HMUrl;

        private List<string> highPrioUpdates = new List<string>();
        public List<string> HighPrioUpdates { get { return highPrioUpdates; } }

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
                UpdateStates(false);
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
            // requesting system variables list from HomeMatic XmlApi
            XmlDocument xmlVariables = GetApiData(xmlApiMethodVariable);

            // clear current collection
            variables.Clear();

            // iterating variables
            foreach (XmlElement varElement in xmlVariables.DocumentElement.ChildNodes)
            {
                HMSystemVariable variable = new HMSystemVariable()
                {
                    InternalId = int.Parse(varElement.GetAttribute("ise_id")),
                    Name = varElement.GetAttribute("name"),
                    Type = varElement.GetAttribute("type"),
                    Value = varElement.GetAttribute("value"),
                    LastUpdateTimeStamp = long.Parse(varElement.GetAttribute("timestamp"))
                };

                variables.Add(variable);
            }
        }

        /// <summary>
        /// Updates the global list of system messages
        /// </summary>
        public void UpdateMessages()
        {
            // requesting system messages list from HomeMatic XmlApi
            XmlDocument xmlMessages = GetApiData(xmlApiMethodMessage);

            // clear current collection
            messages.Clear();

            // iterating messages
            foreach (XmlElement msgElement in xmlMessages.DocumentElement.ChildNodes)
            {
                HMSystemMessage message = new HMSystemMessage()
                {
                    InternalId = int.Parse(msgElement.GetAttribute("ise_id")),
                    Type = msgElement.GetAttribute("type"),
                    OccuredOnTimeStamp = long.Parse(msgElement.GetAttribute("timestamp"))
                };

                messages.Add(message);
            }
        }

        /// <summary>
        /// Triggers update of the global device list including their channels and data point or state data
        /// <param name="justUpdateHighPrioDevices">Tells the method to just run an short update of an important set of devices</param>
        /// </summary>
        public void UpdateStates(bool justUpdateHighPrioDevices)
        {
            if(justUpdateHighPrioDevices)
            {
                UpdateStates();
                return;
            }

            // requesting states list from HomeMatic XmlApi
            XmlDocument xmlStates = GetApiData(xmlApiMethodStateAll);
            
            if (xmlStates != null)
            {
                UpdateStates(xmlStates);
            }
        }

        /// <summary>
        /// Triggers update of the global device list just for highly priorized devices including their channels and data point or state data
        /// </summary>
        private void UpdateStates()
        {
            string queryIds = String.Empty;

            foreach(string address in highPrioUpdates)
            {
                queryIds += GetInternalIdByAddress(address) + ",";
            }

            // requesting states list from HomeMatic XmlApi
            XmlDocument xmlStates = GetApiData(xmlApiMethodStateSingle, "device_id", queryIds);

            if (xmlStates != null)
            {
                UpdateStates(xmlStates);
            }
        }

        /// <summary>
        /// Updates the global device list including their channels and data point or state data based on XML input
        /// </summary>
        /// <param name="xmlStates">XML data for states</param>
        private void UpdateStates(XmlDocument xmlStates)
        {
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

                                    channel.AddDataPoint(dataPoint.Type, dataPoint);
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

        #region Class helper

        /// <summary>
        /// Request an XML based API document from HomeMatic Xml Api
        /// </summary>
        /// <param name="apiMethod">Name of the method to call</param>
        /// <returns>XML document containing the requested data</returns>
        private XmlDocument GetApiData(string apiMethod)
        {
            XmlDocument result = new XmlDocument();

            if (HMUrl != null)
            {
                WebClient apiClient = new WebClient();
                result.LoadXml(apiClient.DownloadString(String.Format("{0}{1}/{2}.cgi", HMUrl, xmlApiDefaultPath, apiMethod)));

                if (result != null && result.DocumentElement != null)
                {
                    return result;
                }
            }

            return null;
        }

        /// <summary>
        /// Request an XML based API document from HomeMatic Xml Api
        /// </summary>
        /// <param name="apiMethod">Name of the method to call</param>
        /// <param name="parameter">Name of the parameter to attach</param>
        /// <param name="parameterValue">Value of the parameter to attach</param>
        /// <returns>XML document containing the requested data</returns>
        private XmlDocument GetApiData(string apiMethod, string parameter, string parameterValue)
        {
            XmlDocument result = new XmlDocument();

            if (HMUrl != null)
            {
                WebClient apiClient = new WebClient();
                result.LoadXml(apiClient.DownloadString(String.Format("{0}{1}/{2}.cgi?{3}={4}", HMUrl, xmlApiDefaultPath, apiMethod, parameter, parameterValue)));

                if (result != null && result.DocumentElement != null)
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

            try
            {
                bool searchChannels = address.Contains(":");

                HMDevice device = null;
                HMDeviceChannel channel = null;

                device = devices.First(d => address.StartsWith(d.Address));

                if (searchChannels && device != null)
                {
                    channel = device.Channels.First(c => c.Address == address);
                }

                if (device != null && !String.IsNullOrEmpty(device.Address))
                {
                    result = device.InternalId;

                    if (channel != null && !String.IsNullOrEmpty(channel.Address))
                    {
                        result = channel.InternalId;
                    }
                }
            }
            catch(Exception)
            {
                // Seems that we have a problem finding this id in internal data so we let it go and pass a minus-one to indicate our disability
            }

            return result;
        }

        /// <summary>
        /// Gets the device by given HomeMatic device or channel address
        /// </summary>
        /// <param name="address">HomeMatic device or channel address</param>
        /// <returns>Device</returns>
        private HMDevice GetDeviceByAddress(string address)
        {
            return devices.First(d => address.StartsWith(d.Address));
        }

        /// <summary>
        /// Gets the device by given HomeMatic device or channel address
        /// </summary>
        /// <param name="address">HomeMatic device or channel address</param>
        /// <returns>Device</returns>
        private HMDeviceChannel GetChannelByAddress(string address)
        {
            if (address.Contains(":"))
            {
                HMDevice device = GetDeviceByAddress(address);
                if(device != null && device.Channels.Count > 0)
                {
                    return device.Channels.First(c => c.Address == address);
                }
            }

            return null;
        }

        /// <summary>
        /// Adds devices by address to the list for high priorized updates, which will be performed faster 
        /// by requesting them by single call than getting all status values 
        /// </summary>
        /// <param name="address"></param>
        public void SetupHighPrioDevice(string address)
        {
            if (!String.IsNullOrWhiteSpace(address))
            {
                if (address.Contains(":"))
                {
                    highPrioUpdates.Add(address.Substring(0, address.IndexOf(":")));
                }
                else
                {
                    highPrioUpdates.Add(address);
                }
            }
        }

        /// <summary>
        /// Clears the list for high priorized updates
        /// </summary>
        public void ClearHighPrioDevices()
        {
            highPrioUpdates.Clear();
        }

        #endregion

        #region Common helper

        /// <summary>
        /// Converts UNIX timestamp to valid DateTime
        /// </summary>
        /// <param name="timeStamp">UNIX timestamp</param>
        /// <returns>DateTime object representing the given UNIX timestamp</returns>
        public static DateTime TimeStampToDateTime(long timeStamp)
        {
            if (timeStamp > 1)
            {
                return (new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local)).AddSeconds(timeStamp);
            }

            return DateTime.MinValue;
        }

        #endregion
    }
}
