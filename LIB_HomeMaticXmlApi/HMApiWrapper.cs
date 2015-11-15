using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Xml;

namespace TRoschinsky.Lib.HomeMaticXmlApi
{
    /// <summary>
    /// The Homematic XML-API wrapper core. This class provides you with all the essential methods 
    /// to talk to a Homematic CCU2 with XML-API v1.10+ add-on installed. Once connected successfully 
    /// to the CCU2 you're able to read and set all the devices including all channels and its data points.
    /// The CCU2s service messages and system variables can be read and cleared/set as well.
    /// The API wrapper needs to be triggered to take any actions - you'll have to take care of 
    /// refreshing status values yourself. Due to performance reasons you can choose to simply just 
    /// refresh some explicitly needed devices by adding them to the FastUpdateDevices collection.
    /// </summary>
    public class HMApiWrapper
    {
        private string xmlApiDefaultPath = "addons/xmlapi";

        private string xmlApiMethodDevice = "devicelist";
        private string xmlApiMethodStateAll = "statelist";
        private string xmlApiMethodStateSingle = "state";
        private string xmlApiMethodStateSet = "statechange";
        private string xmlApiMethodVariable = "sysvarlist";
        private string xmlApiMethodVariableSet = "statechange";
        private string xmlApiMethodMessage = "systemNotification";
        private string xmlApiMethodMessageSet = "systemNotificationClear";

        private List<string> log = new List<string>();
        public string[] Log { get { return log.ToArray(); } }

        private Uri HMUrl;

        private List<string> fastUpdateDevices = new List<string>();
        public List<string> FastUpdateDevices { get { return fastUpdateDevices; } }

        private List<HMSystemVariable> variables = new List<HMSystemVariable>();
        public List<HMSystemVariable> Variables { get { return variables; } }

        private List<HMSystemMessage> messages = new List<HMSystemMessage>();
        public List<HMSystemMessage> Messages { get { return messages; } }

        private List<HMDevice> devices = new List<HMDevice>();
        public List<HMDevice> Devices { get { return devices; } }


        /// <summary>
        /// Basic constructor of Homematic wrapper. Connects to the XML-API on CCU2 and triggers initialization 
        /// without retrieving states, variables or messages.
        /// </summary>
        /// <param name="homeMaticUri">Uri to HomeMatic</param>
        public HMApiWrapper(Uri homeMaticUri)
        {
            HMUrl = homeMaticUri;
            Initialize(false, false);
        }

        /// <summary>
        /// Advanced constructor of Homematic wrapper. Connects to the XML-API on CCU2 and lets you choose 
        /// what types of information (states, variables or messages) you'll initialize
        /// </summary>
        /// <param name="homeMaticUri">Uri to HomeMatic</param>
        /// <param name="initializeWithStates">Set to true if you want to initialize the wrapper with states; operation takes longer but you are able to access states immediately</param>
        /// <param name="initializeWithVariables">Set to true if you want to initialize the wrapper with HomeMatic system variables</param>
        public HMApiWrapper(Uri homeMaticUri, bool initializeWithStates, bool initializeWithVariables)
        {
            HMUrl = homeMaticUri;
            Initialize(initializeWithStates, initializeWithVariables);
        }

        #region Main logic

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
        /// Gets all devices including their channels but without any data point or state data
        /// </summary>
        /// <returns>List containing devices with channels</returns>
        private List<HMDevice> GetDevices()
        {
            List<HMDevice> result = new List<HMDevice>();

            // requesting devices list from HomeMatic XmlApi
            XmlDocument xmlDevices = GetApiData(xmlApiMethodDevice);

            // iterating devices
            foreach (XmlElement devElement in xmlDevices.DocumentElement.ChildNodes)
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

        #region Homematic system variables

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
        /// Sets a referenced system variable to a given value
        /// </summary>
        /// <param name="hmElement">An HMBase object that represents a system variable</param>
        /// <param name="value">The new value the needs to be assigned to the system variable</param>
        /// <returns></returns>
        public bool SetVariable(HMBase hmElement, object value)
        {
            try
            {
                if (hmElement == null || hmElement.GetType() != typeof(HMSystemVariable))
                {
                    return false;
                }

                string internalId = hmElement.InternalId.ToString();
                string stringRepresentationOfValue = Convert.ToString(value).ToLower();

                // sending change of variable request to HomeMatic XmlApi
                XmlDocument xmlSetStates = GetApiData(xmlApiMethodVariableSet, "ise_id", internalId, "new_value", stringRepresentationOfValue);

                // checking results
                XmlNode resultNode = xmlSetStates.DocumentElement.FirstChild;
                {
                    if (resultNode.Name == "changed" && resultNode.Attributes["id"].Value == internalId && resultNode.Attributes["new_value"].Value == stringRepresentationOfValue)
                    {
                        UpdateVariables();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region Homematic system messages

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
                    Name = msgElement.GetAttribute("name"),
                    InternalId = int.Parse(msgElement.GetAttribute("ise_id")),
                    Type = msgElement.GetAttribute("type"),
                    OccuredOnTimeStamp = long.Parse(msgElement.GetAttribute("timestamp"))
                };

                messages.Add(message);
            }
        }

        /// <summary>
        /// Confirms all messages and resets the global list of system messages
        /// </summary>
        public void SetMessages()
        {
            // requesting system messages list from HomeMatic XmlApi
            XmlDocument xmlMessages = GetApiData(xmlApiMethodMessageSet);

            // wait a little while
            System.Threading.Thread.Sleep(250);

            // update messages
            UpdateMessages();
        }

        #endregion

        #region Homematic system states

        /// <summary>
        /// Triggers update of the global device list including their channels and data point or state data
        /// <param name="justQueryForFastUpdateDevices">Tells the method to just run an short update of an important set of devices</param>
        /// </summary>
        public void UpdateStates(bool justQueryForFastUpdateDevices)
        {
            if(justQueryForFastUpdateDevices)
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
        /// Triggers update of the global device list just for devices listed in fast update devices 
        /// collection, including their channels and data point or state data. If no fast update devices 
        /// are defined, we do a regular states update.
        /// </summary>
        private void UpdateStates()
        {
            string queryIds = String.Empty;

            foreach(string address in fastUpdateDevices)
            {
                queryIds += GetInternalIdByAddress(address) + ",";
            }

            if (!String.IsNullOrWhiteSpace(queryIds))
            {
                // requesting states list from HomeMatic XmlApi
                XmlDocument xmlStates = GetApiData(xmlApiMethodStateSingle, "device_id", queryIds);

                if (xmlStates != null)
                {
                    UpdateStates(xmlStates);
                    return;
                }
            }

            // do a regular states update if not run into single update branch above
            UpdateStates(false);
        }

        /// <summary>
        /// Updates the global device list including their channels and data point or state data based on XML input
        /// </summary>
        /// <param name="xmlStates">XML data for states</param>
        private void UpdateStates(XmlDocument xmlStates)
        {
            string currentElementPlain = String.Empty;

            // iterating devices
            foreach (XmlElement devElement in xmlStates.DocumentElement.ChildNodes)
            {
                try
                {
                    currentElementPlain = devElement.InnerXml.Length >= 100 ? devElement.InnerXml.Substring(0, 100) : devElement.InnerXml;

                    int devIseId = int.Parse(devElement.GetAttribute("ise_id"));
                    // looking for existing device
                    HMDevice device = devices.First(d => devIseId == d.InternalId);

                    // iterating channels
                    foreach (XmlElement chanElement in devElement.ChildNodes)
                    {
                        try
                        {
                            currentElementPlain = chanElement.InnerXml.Length >= 100 ? chanElement.InnerXml.Substring(0, 100) : chanElement.InnerXml;

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
                                    try
                                    {
                                        HMDeviceDataPoint dataPoint = new HMDeviceDataPoint()
                                        {
                                            InternalId = int.Parse(pointElement.GetAttribute("ise_id")),
                                            InternalIdParent = chanIseId,
                                            Type = pointElement.GetAttribute("type"),
                                            LastUpdateTimeStamp = long.Parse(pointElement.GetAttribute("timestamp")),
                                            ValueString = pointElement.GetAttribute("value"),
                                            ValueType = pointElement.GetAttribute("valuetype"),
                                            ValueUnit = pointElement.GetAttribute("valueunit")
                                        };
                                        channel.AddDataPoint(dataPoint.Type, dataPoint);
                                    }
                                    catch (Exception ex)
                                    {
                                        WriteInternalLog("DataPoint failed: " + ex.Message, true);
                                        // well, maybe there was an datapoint that could not be created 
                                        // due to missing information
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            WriteInternalLog("Channel failed: " + ex.Message + "\n  --- " + currentElementPlain, true);
                            // well, maybe there was an channel that is not listed in device list
                            // no problem, we'll just ignore it at this point
                        }
                    }
                }
                catch (Exception ex)
                {
                    WriteInternalLog("Device failed: " + ex.Message + "\n  --- " + currentElementPlain, true);
                    // well, maybe there was an device that is not listed in device list
                    // no problem, we'll just ignore it at this point
                }
            }
        }

        /// <summary>
        /// Triggers update of the global device list just for a single device by given HM element
        /// including its channels and data point or state data.
        /// </summary>
        private void UpdateState(HMBase hmElement)
        {
            if (hmElement != null)
            {
                // to update a single state its important to know what type we have to update
                string param = String.Empty;
                int iseId = 0;

                switch (hmElement.GetType().Name)
                {
                    case "HMDevice":
                        param = "device_id";
                        iseId = hmElement.InternalId;
                        break;

                    case "HMDeviceChannel":
                        param = "channel_id";
                        iseId = hmElement.InternalId;
                        break;

                    case "HMDeviceDataPoint":
                        param = "channel_id";
                        iseId = ((HMDeviceDataPoint)hmElement).InternalIdParent;
                        break;

                    default:
                        break;
                }

                // requesting states list from HomeMatic XmlApi
                XmlDocument xmlStates = GetApiData(xmlApiMethodStateSingle, param, iseId.ToString());

                if (xmlStates != null)
                {
                    UpdateStates(xmlStates);
                    return;
                }
            }
        }

        /// <summary>
        /// Sets the state of a data point by address of the data point
        /// </summary>
        /// <param name="address">Address of channel</param>
        /// <param name="value">The new value the needs to be assigned to the data point</param>
        /// <returns>Result of operation; true if everything is okay</returns>
        /// <remarks>Updating of set element is not supported (see deprecated message)!</remarks>
        [Obsolete("Please use the SetState(HMBase, object) or the SetStateByAddress(string, string, object) method!", false)]
        public bool SetState(string address, object value)
        {
            // We're just able to set the primary datapoint because we're not addressing the datapoint by key
            return SetState(GetInternalIdByAddress(address), value);
        }

        /// <summary>
        /// Sets the state of a data point by internal ID of the data point
        /// </summary>
        /// <param name="internalId">IseId of the data point to set</param>
        /// <param name="value">The new value the needs to be assigned to the data point</param>
        /// <returns>Result of operation; true if everything is okay</returns>
        /// <remarks>Updating of set element is not supported (see deprecated message)!</remarks>
        [Obsolete("Please use the SetState(HMBase, object) or the SetStateByAddress(string, string, object) method!", false)]
        public bool SetState(int internalId, object value)
        {
            try
            {
                if(internalId <= 0 || value == null)
                {
                    return false;
                }

                string iseId = internalId.ToString();
                string stringRepresentationOfValue = Convert.ToString(value).ToLower();

                // sending change of state request to HomeMatic XmlApi
                XmlDocument xmlSetStates = GetApiData(xmlApiMethodStateSet, "ise_id", iseId, "new_value", stringRepresentationOfValue);

                // checking results
                XmlNode resultNode = xmlSetStates.DocumentElement.FirstChild;
                {
                    if(resultNode.Name == "changed" && resultNode.Attributes["id"].Value == iseId && resultNode.Attributes["new_value"].Value == stringRepresentationOfValue)
                    {
                        // No internal updating implemented because we're just aware of the internal id but we did not know if it is a channel or datapoint
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch 
            {
                return false;
            }
        }

        /// <summary>
        /// Sets the state of a data point by address of the data point
        /// </summary>
        /// <param name="hmElement">An HMBase object that represents a channel or a data point</param>
        /// <param name="value">The new value the needs to be assigned to the data point</param>
        /// <returns>Result of operation; true if everything is okay</returns>
        public bool SetState(HMBase hmElement, object value)
        {
            try
            {
                if (hmElement == null || !(hmElement.GetType() == typeof(HMDeviceChannel) || hmElement.GetType() == typeof(HMDeviceDataPoint)))
                {
                    return false;
                }

                string internalId = hmElement.InternalId.ToString();
                string stringRepresentationOfValue = Convert.ToString(value).ToLower();

                // sending change of state request to HomeMatic XmlApi
                XmlDocument xmlSetStates = GetApiData(xmlApiMethodStateSet, "ise_id", internalId, "new_value", stringRepresentationOfValue);

                // checking results
                XmlNode resultNode = xmlSetStates.DocumentElement.FirstChild;
                {
                    if (resultNode.Name == "changed" && resultNode.Attributes["id"].Value == internalId && resultNode.Attributes["new_value"].Value == stringRepresentationOfValue)
                    {
                        UpdateState(hmElement);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Sets the state of a data point by full address and key of the data point
        /// </summary>
        /// <param name="address">Full address of the desired channel</param>
        /// <param name="key">Key or typename of the desired data point</param>
        /// <param name="Value">The new value the needs to be assigned to the data point</param>
        /// <returns>Result of operation; true if everything is okay</returns>
        public bool SetStateByAddress(string address, string key, object Value)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(address) || String.IsNullOrWhiteSpace(key) || Value == null)
                {
                    return false;
                }

                return SetState(GetDataByAddress(address, key), Value);
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #endregion

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
        /// Request an XML based API document from HomeMatic Xml Api with one parameter
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
        /// Request an XML based API document from HomeMatic Xml Api with two parameters
        /// </summary>
        /// <param name="apiMethod">Name of the method to call</param>
        /// <param name="parameter1">Name of the parameter #1 to attach</param>
        /// <param name="parameterValue1">Value of the parameter #1 to attach</param>
        /// <param name="parameter2">Name of the parameter #2 to attach</param>
        /// <param name="parameterValue2">Value of the parameter #2 to attach</param>
        /// <returns>XML document containing the requested data</returns>
        private XmlDocument GetApiData(string apiMethod, string parameter1, string parameterValue1, string parameter2, string parameterValue2)
        {
            XmlDocument result = new XmlDocument(); 

            if (HMUrl != null)
            {
                WebClient apiClient = new WebClient();
                result.LoadXml(apiClient.DownloadString(String.Format("{0}{1}/{2}.cgi?{3}={4}&{5}={6}", HMUrl, xmlApiDefaultPath, apiMethod, parameter1, parameterValue1, parameter2, parameterValue2)));

                if (result != null && result.DocumentElement != null)
                {
                    return result;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the device by given HomeMatic device or channel address
        /// </summary>
        /// <param name="address">HomeMatic device or channel address</param>
        /// <returns>Device</returns>
        public HMDevice GetDeviceByAddress(string address)
        {
            try
            {
                if (address.Contains(":"))
                {
                    return devices.First(d => d.Address == (address.Substring(0, address.IndexOf(":"))));
                }
                else
                {
                    return devices.First(d => d.Address == address);
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the device channel by given HomeMatic channel address
        /// </summary>
        /// <param name="address">HomeMatic channel address (ends with channel number separated by ':')</param>
        /// <returns>Channel</returns>
        public HMDeviceChannel GetChannelByAddress(string address)
        {
            try
            {
                if (address.Contains(":"))
                {
                    HMDevice device = GetDeviceByAddress(address);
                    if (device != null && device.Channels.Count > 0)
                    {
                        return device.Channels.First(c => c.Address == address);
                    }
                }

                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the device channels datapoint by given HomeMatic device or channel address and data value type name
        /// </summary>
        /// <param name="address"></param>
        /// <param name="valueType">The name of the value type (STATE, LOWBAT, MOTION, etc.)</param>
        /// <returns>Data point</returns>
        public HMDeviceDataPoint GetDataByAddress(string address, string valueTypeName)
        {
            try
            {
                HMDeviceChannel channel = GetChannelByAddress(address);
                if (channel != null && channel.DataPoints.ContainsKey(valueTypeName))
                {
                    return channel.DataPoints[valueTypeName];
                }

                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the internal ID for devices or channels by given HomeMatic device or channel address
        /// </summary>
        /// <param name="address">HomeMatic device or channel address</param>
        /// <returns>Internal ID (iseId); if it is -1 we wasn't able to find a matching device or channel</returns>
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
        /// Adds devices by address to the list for single updates, which will be performed faster 
        /// by requesting them by single call than getting all status values 
        /// </summary>
        /// <param name="address"></param>
        public void FastUpdateDeviceSetup(string address)
        {
            if (!String.IsNullOrWhiteSpace(address))
            {
                if (address.Contains(":"))
                {
                    fastUpdateDevices.Add(address.Substring(0, address.IndexOf(":")));
                }
                else
                {
                    fastUpdateDevices.Add(address);
                }
            }
        }

        /// <summary>
        /// Clears the list for fast update devices
        /// </summary>
        public void FastUpdateDevicesClear()
        {
            fastUpdateDevices.Clear();
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

        private void WriteInternalLog(string message, bool isError)
        {
            log.Add(String.Format("{0}-[{1}]: {2}", DateTime.Now, isError ? "ERR" : "INF", message));
        }

        #endregion
    }
}
