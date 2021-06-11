using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;

// ReSharper disable UnusedMember.Global

// ReSharper disable InconsistentNaming

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
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public partial class HMApiWrapper
    {
        private static readonly HttpClient Client = new HttpClient();

        private const string xmlApiDefaultPath = "addons/xmlapi";
        private const string xmlApiMethodDevice = "devicelist";
        private const string xmlApiMethodStateAll = "statelist";
        private const string xmlApiMethodStateSingle = "state";
        private const string xmlApiMethodStateSet = "statechange";
        private const string xmlApiMethodVariable = "sysvarlist";
        private const string xmlApiMethodVariableSet = "statechange";
        private const string xmlApiMethodMessage = "systemNotification";
        private const string xmlApiMethodMessageSet = "systemNotificationClear";

        private readonly List<string> log = new List<string>();

        public string[] Log => log.ToArray();

        public Uri HmUrl { get; }

        public List<string> FastUpdateDevices { get; } = new List<string>();

        public List<HMSystemVariable> Variables { get; } = new List<HMSystemVariable>();

        public List<HMSystemMessage> Messages { get; } = new List<HMSystemMessage>();

        public List<HMDevice> Devices { get; private set; } = new List<HMDevice>();

        private bool _initialized;


        /// <summary>
        /// Basic constructor of Homematic wrapper
        /// </summary>
        /// <param name="homeMaticUri">Uri to HomeMatic</param>
        public HMApiWrapper(Uri homeMaticUri)
        {
            HmUrl = homeMaticUri;
        }

        #region Main logic

        /// <summary>
        /// Method to initialize the wrapper; gets all available devices and, if needed, states of devices
        /// </summary>
        /// <param name="initializeWithStates">Set to true if you want to initialize the wrapper with states; operation takes longer but you are able to access states immediately</param>
        /// <param name="variablesInit">Set to true if you want to initialize the wrapper with HomeMatic system variables</param>
        public async Task InitializeAsync(bool initializeWithStates = false, bool variablesInit = false)
        {
            if (_initialized)
                return;

            Devices = await GetDevicesAsync();

            if (initializeWithStates)
                await UpdateStatesAsync(false);

            if (variablesInit)
                await UpdateVariablesAsync();

            _initialized = true;
        }

        /// <summary>
        /// Gets all devices including their channels but without any data point or state data
        /// </summary>
        /// <returns>List containing devices with channels</returns>
        private async Task<List<HMDevice>> GetDevicesAsync()
        {
            var result = new List<HMDevice>();

            // requesting devices list from HomeMatic XmlApi
            var xmlDevices = await GetApiDataAsync(xmlApiMethodDevice);
            if (xmlDevices?.DocumentElement == null)
                return result;

            // iterating devices
            foreach (XmlElement devElement in xmlDevices.DocumentElement.ChildNodes)
            {
                var device = new HMDevice()
                {
                    Name = devElement.GetAttribute("name"),
                    Address = devElement.GetAttribute("address"),
                    InternalId = int.Parse(devElement.GetAttribute("ise_id")),
                    DeviceType = devElement.GetAttribute("device_type")
                };

                // iterating channels
                foreach (XmlElement chanElement in devElement.ChildNodes)
                {
                    var channel = new HMDeviceChannel()
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
        /// <exception cref="HMApiException"><see cref="HMApiWrapper"/> is not initialized</exception>
        public async Task UpdateVariablesAsync()
        {
            CheckInitialized();

            // requesting system variables list from HomeMatic XmlApi
            var xmlVariables = await GetApiDataAsync(xmlApiMethodVariable);

            if (xmlVariables?.DocumentElement == null)
                return;

            // clear current collection
            Variables.Clear();

            // iterating variables
            foreach (XmlElement varElement in xmlVariables.DocumentElement.ChildNodes)
            {
                try
                {
                    var variable = new HMSystemVariable()
                    {
                        InternalId = int.Parse(varElement.GetAttribute("ise_id")),
                        Name = varElement.GetAttribute("name"),
                        ValueType = varElement.GetAttribute("type"),
                        ValueUnit = varElement.GetAttribute("unit"),
                        ValueString = varElement.GetAttribute("value"),
                        IsLogged = bool.Parse(varElement.GetAttribute("logged")),
                        IsVisible = bool.Parse(varElement.GetAttribute("visible")),
                        LastUpdateTimeStamp = long.Parse(varElement.GetAttribute("timestamp"))
                    };

                    if (!string.IsNullOrEmpty(variable.ValueType))
                    {
                        switch (int.Parse(variable.ValueType))
                        {
                            case 16:
                                variable.SetValuesIndex(varElement.GetAttribute("value_list"));
                                break;
                            case 2:
                                variable.SetValuesIndex(varElement.GetAttribute("value_name_1"), varElement.GetAttribute("value_name_0"));
                                break;
                        }
                    }

                    Variables.Add(variable);
                }
                catch (Exception)
                {
                    // Ignore mismatched variable
                }
            }
        }

        /// <summary>
        /// Sets a referenced system variable to a given value
        /// </summary>
        /// <param name="hmElement">An HMBase object that represents a system variable</param>
        /// <param name="value">The new value the needs to be assigned to the system variable</param>
        /// <returns></returns>
        /// <exception cref="HMApiException"><see cref="HMApiWrapper"/> is not initialized</exception>
        public async Task<bool> SetVariableAsync(HMBase hmElement, object value)
        {
            CheckInitialized();

            try
            {
                if (hmElement == null || hmElement.GetType() != typeof(HMSystemVariable))
                    return false;

                var internalId = hmElement.InternalId.ToString();
                var stringRepresentationOfValue = Convert.ToString(value).ToLower();

                // sending change of variable request to HomeMatic XmlApi
                var xmlSetStates = await GetApiDataAsync(xmlApiMethodVariableSet, "ise_id", internalId, "new_value", stringRepresentationOfValue);

                if (xmlSetStates?.DocumentElement == null)
                    return false;

                // checking results
                var resultNode = xmlSetStates.DocumentElement.FirstChild;
                {
                    if (resultNode.Name != "changed" || resultNode.Attributes?["id"].Value != internalId || resultNode.Attributes["new_value"].Value != stringRepresentationOfValue)
                        return false;

                    await UpdateVariablesAsync();
                    return true;
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
        /// <exception cref="HMApiException"><see cref="HMApiWrapper"/> is not initialized</exception>
        public async Task UpdateMessagesAsync()
        {
            CheckInitialized();

            // requesting system messages list from HomeMatic XmlApi
            var xmlMessages = await GetApiDataAsync(xmlApiMethodMessage);

            if (xmlMessages?.DocumentElement == null)
                return;

            // clear current collection
            Messages.Clear();

            // iterating messages
            foreach (XmlElement msgElement in xmlMessages.DocumentElement.ChildNodes)
            {
                var message = new HMSystemMessage()
                {
                    Name = msgElement.GetAttribute("name"),
                    InternalId = int.Parse(msgElement.GetAttribute("ise_id")),
                    Type = msgElement.GetAttribute("type"),
                    OccurredOnTimeStamp = long.Parse(msgElement.GetAttribute("timestamp"))
                };

                Messages.Add(message);
            }
        }

        /// <summary>
        /// Confirms all messages and resets the global list of system messages
        /// </summary>
        /// <exception cref="HMApiException"><see cref="HMApiWrapper"/> is not initialized</exception>
        public async Task SetMessagesAsync()
        {
            CheckInitialized();

            // requesting system messages list from HomeMatic XmlApi
            await GetApiDataAsync(xmlApiMethodMessageSet);

            // wait a little while
            await Task.Delay(250);

            // update messages
            await UpdateMessagesAsync();
        }

        #endregion

        #region Homematic system states

        /// <summary>
        /// Triggers update of the global device list including their channels and data point or state data
        /// <param name="justQueryForFastUpdateDevices">Tells the method to just run an short update of an important set of devices</param>
        /// </summary>
        /// <exception cref="HMApiException"><see cref="HMApiWrapper"/> is not initialized</exception>
        public async Task UpdateStatesAsync(bool justQueryForFastUpdateDevices)
        {
            CheckInitialized();

            if (justQueryForFastUpdateDevices)
            {
                await UpdateStatesAsync();
                return;
            }

            // requesting states list from HomeMatic XmlApi
            var xmlStates = await GetApiDataAsync(xmlApiMethodStateAll);

            if (xmlStates != null)
                UpdateStatesAsync(xmlStates);
        }

        /// <summary>
        /// Triggers update of the global device list just for devices listed in fast update devices
        /// collection, including their channels and data point or state data. If no fast update devices
        /// are defined, we do a regular states update.
        /// </summary>
        private async Task UpdateStatesAsync()
        {
            var queryIds = string.Empty;

            foreach (var address in FastUpdateDevices)
            {
                queryIds += GetInternalIdByAddress(address) + ",";
            }

            if (!string.IsNullOrWhiteSpace(queryIds))
            {
                // requesting states list from HomeMatic XmlApi
                var xmlStates = await GetApiDataAsync(xmlApiMethodStateSingle, "device_id", queryIds);

                if (xmlStates != null)
                {
                    UpdateStatesAsync(xmlStates);
                    return;
                }
            }

            // do a regular states update if not run into single update branch above
            await UpdateStatesAsync(false);
        }

        /// <summary>
        /// Updates the global device list including their channels and data point or state data based on XML input
        /// </summary>
        /// <param name="xmlStates">XML data for states</param>
        private void UpdateStatesAsync(XmlDocument xmlStates)
        {
            if (xmlStates?.DocumentElement == null)
                return;

            var currentElementPlain = string.Empty;

            // iterating devices
            foreach (XmlElement devElement in xmlStates.DocumentElement.ChildNodes)
            {
                try
                {
                    currentElementPlain = devElement.InnerXml.Length >= 100 ? devElement.InnerXml.Substring(0, 100) : devElement.InnerXml;

                    var devIseId = int.Parse(devElement.GetAttribute("ise_id"));
                    // looking for existing device
                    HMDevice device;

                    try
                    {
                        device = Devices.First(d => devIseId == d.InternalId);
                    }
                    catch
                    {
                        continue;
                    }

                    // iterating channels
                    foreach (XmlElement chanElement in devElement.ChildNodes)
                    {
                        try
                        {
                            currentElementPlain = chanElement.InnerXml.Length >= 100 ? chanElement.InnerXml.Substring(0, 100) : chanElement.InnerXml;

                            var chanIseId = int.Parse(chanElement.GetAttribute("ise_id"));

                            // looking for existing channel
                            HMDeviceChannel channel;

                            try
                            {
                                channel = device.Channels.First(c => chanIseId == c.InternalId);
                            }
                            catch
                            {
                                channel = null;
                            }

                            if (channel == null && chanElement.GetAttribute("name").Contains(device.Name + ":0"))
                            {
                                // create new channel and add to device
                                channel = new HMDeviceChannel()
                                {
                                    Name = "DeviceRoot",
                                    Address = string.Concat(device.Address, ":0"),
                                    InternalId = int.Parse(chanElement.GetAttribute("ise_id")),
                                };
                                device.AddChannel(channel);
                            }
                            else if (channel == null)
                            {
                                // create new channel and add to device
                                channel = new HMDeviceChannel()
                                {
                                    Name = chanElement.GetAttribute("name"),
                                    Address = chanElement.GetAttribute("address"),
                                    InternalId = int.Parse(chanElement.GetAttribute("ise_id")),
                                };
                                device.AddChannel(channel);
                            }
                            else
                            {
                                // clear all data points to create new
                                channel.DataPoints.Clear();
                            }

                            // iterating data points
                            foreach (XmlElement pointElement in chanElement.ChildNodes)
                            {
                                try
                                {
                                    var dataPoint = new HMDeviceDataPoint
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
        private async Task UpdateStateAsync(HMBase hmElement)
        {
            if (hmElement == null)
                return;
            // to update a single state its important to know what type we have to update
            var param = string.Empty;
            var iseId = 0;

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
                    iseId = ((HMDeviceDataPoint) hmElement).InternalIdParent;
                    break;
            }

            // requesting states list from HomeMatic XmlApi
            var xmlStates = await GetApiDataAsync(xmlApiMethodStateSingle, param, iseId.ToString());

            if (xmlStates != null)
                UpdateStatesAsync(xmlStates);
        }

        /// <summary>
        /// Sets the state of a data point by address of the data point
        /// </summary>
        /// <param name="hmElement">An HMBase object that represents a channel or a data point</param>
        /// <param name="value">The new value the needs to be assigned to the data point</param>
        /// <returns>Result of operation; true if everything is okay</returns>
        /// <exception cref="HMApiException"><see cref="HMApiWrapper"/> is not initialized</exception>
        public async Task<bool> SetStateAsync(HMBase hmElement, object value)
        {
            CheckInitialized();

            try
            {
                if (hmElement == null || !(hmElement.GetType() == typeof(HMDeviceChannel) || hmElement.GetType() == typeof(HMDeviceDataPoint)))
                    return false;

                var internalId = hmElement.InternalId.ToString();

                var numFormat = new NumberFormatInfo {NumberDecimalSeparator = "."};
                var stringRepresentationOfValue = Convert.ToString(value, numFormat)?.ToLower() ?? "null";

                // sending change of state request to HomeMatic XmlApi
                var xmlSetStates = await GetApiDataAsync(xmlApiMethodStateSet, "ise_id", internalId, "new_value", stringRepresentationOfValue);

                // checking results
                var resultNode = xmlSetStates?.DocumentElement?.FirstChild;
                if (resultNode == null)
                    return false;

                if (resultNode.Name != "changed" || resultNode.Attributes?["id"].Value != internalId || resultNode.Attributes["new_value"].Value != stringRepresentationOfValue)
                    return false;

                await UpdateStateAsync(hmElement);
                return true;
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
        /// <param name="value">The new value the needs to be assigned to the data point</param>
        /// <returns>Result of operation; true if everything is okay</returns>
        /// <exception cref="HMApiException"><see cref="HMApiWrapper"/> is not initialized</exception>
        public async Task<bool> SetStateByAddressAsync(string address, string key, object value)
        {
            CheckInitialized();

            try
            {
                if (string.IsNullOrWhiteSpace(address) || string.IsNullOrWhiteSpace(key) || value == null)
                    return false;

                return await SetStateAsync(GetDataByAddress(address, key), value);
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
        private Task<XmlDocument> GetApiDataAsync(string apiMethod)
            => HmUrl == null ? null : FetchXmlFromUriAsync($"{HmUrl}{xmlApiDefaultPath}/{apiMethod}.cgi");

        /// <summary>
        /// Request an XML based API document from HomeMatic Xml Api with one parameter
        /// </summary>
        /// <param name="apiMethod">Name of the method to call</param>
        /// <param name="parameter">Name of the parameter to attach</param>
        /// <param name="parameterValue">Value of the parameter to attach</param>
        /// <returns>XML document containing the requested data</returns>
        private Task<XmlDocument> GetApiDataAsync(string apiMethod, string parameter, string parameterValue)
            => HmUrl == null ? null : FetchXmlFromUriAsync($"{HmUrl}{xmlApiDefaultPath}/{apiMethod}.cgi?{parameter}={parameterValue}");

        /// <summary>
        /// Request an XML based API document from HomeMatic Xml Api with two parameters
        /// </summary>
        /// <param name="apiMethod">Name of the method to call</param>
        /// <param name="parameter1">Name of the parameter #1 to attach</param>
        /// <param name="parameterValue1">Value of the parameter #1 to attach</param>
        /// <param name="parameter2">Name of the parameter #2 to attach</param>
        /// <param name="parameterValue2">Value of the parameter #2 to attach</param>
        /// <returns>XML document containing the requested data</returns>
        private Task<XmlDocument> GetApiDataAsync(string apiMethod, string parameter1, string parameterValue1, string parameter2, string parameterValue2)
            => HmUrl == null ? null : FetchXmlFromUriAsync($"{HmUrl}{xmlApiDefaultPath}/{apiMethod}.cgi?{parameter1}={parameterValue1}&{parameter2}={parameterValue2}");

        /// <summary>
        /// Fetch content from given url and parse to xml
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        private async Task<XmlDocument> FetchXmlFromUriAsync(string uri)
        {
            if (string.IsNullOrEmpty(uri))
                return null;

            var result = new XmlDocument();

            if (HmUrl == null)
                return null;

            var plainTextResponse = await Client.GetStringAsync(uri);

            result.LoadXml(plainTextResponse);

            return result.DocumentElement != null ? result : null;
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
                var addressParts = address.Split(':');
                return Devices.First(d => d.Address == addressParts[0]);
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
                if (!address.Contains(":"))
                    return null;

                var device = GetDeviceByAddress(address);

                if (device != null && device.Channels.Count > 0)
                    return device.Channels.First(c => c.Address == address);

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
        /// <param name="valueTypeName">The name of the value type (STATE, LOWBAT, MOTION, etc.)</param>
        /// <returns>Data point</returns>
        public HMDeviceDataPoint GetDataByAddress(string address, string valueTypeName)
        {
            try
            {
                var channel = GetChannelByAddress(address);

                if (channel != null && channel.DataPoints.ContainsKey(valueTypeName))
                    return channel.DataPoints[valueTypeName];

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
            var result = -1;

            try
            {
                var searchChannels = address.Contains(":");

                HMDeviceChannel channel = null;

                var device = Devices.FirstOrDefault(d => address.StartsWith(d.Address));
                if (device == null || string.IsNullOrEmpty(device.Address))
                    return result;

                if (searchChannels)
                    channel = device.Channels.First(c => c.Address == address);

                result = device.InternalId;

                if (channel != null && !string.IsNullOrEmpty(channel.Address))
                    result = channel.InternalId;
            }
            catch (Exception)
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
            if (string.IsNullOrWhiteSpace(address))
                return;

            var addressParts = address.Split(':');
            FastUpdateDevices.Add(addressParts[0]);
        }

        /// <summary>
        /// Clears the list for fast update devices
        /// </summary>
        public void FastUpdateDevicesClear()
            => FastUpdateDevices.Clear();

        /// <summary>
        /// Checks if the class is initialized
        /// </summary>
        /// <exception cref="HMApiException"><see cref="HMApiWrapper"/> is not initialized</exception>
        private void CheckInitialized()
        {
            if (!_initialized)
                throw new HMApiException($"{nameof(HMApiWrapper)} is not initialized. Call {nameof(InitializeAsync)} first", "NOT_INITIALIZED");
        }

        #endregion

        #region Common helper

        /// <summary>
        /// Converts UNIX timestamp to valid DateTime
        /// </summary>
        /// <param name="timeStamp">UNIX timestamp</param>
        /// <returns>DateTime object representing the given UNIX timestamp</returns>
        public static DateTime TimeStampToDateTime(long timeStamp)
            => timeStamp > 1 ? new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local).AddSeconds(timeStamp) : DateTime.MinValue;

        private void WriteInternalLog(string message, bool isError)
            => log.Add($"{DateTime.Now}-[{(isError ? "ERR" : "INF")}]: {message}");

        #endregion
    }
}
