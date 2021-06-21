using TRoschinsky.Lib.HomeMaticXmlApi.Helpers;

namespace TRoschinsky.Lib.HomeMaticXmlApi
{
    /// <summary>
    /// Sync overloads for public async methods
    /// </summary>
    public partial class HMApiWrapper
    {
        #region Main logic

        /// <summary>
        /// Method to initialize the wrapper; gets all available devices and, if needed, states of devices
        /// </summary>
        /// <param name="initializeWithStates">Set to true if you want to initialize the wrapper with states; operation takes longer but you are able to access states immediately</param>
        /// <param name="variablesInit">Set to true if you want to initialize the wrapper with HomeMatic system variables</param>
        public void Initialize(bool initializeWithStates = false, bool variablesInit = false)
            => AsyncHelper.RunSync(() => InitializeAsync(initializeWithStates, variablesInit));

        #region Homematic system variables

        /// <summary>
        /// Updates the global list of system variables
        /// </summary>
        /// <exception cref="HMApiException"><see cref="HMApiWrapper"/> is not initialized</exception>
        public void UpdateVariables()
            => AsyncHelper.RunSync(UpdateVariablesAsync);

        /// <summary>
        /// Sets a referenced system variable to a given value
        /// </summary>
        /// <param name="hmElement">An HMBase object that represents a system variable</param>
        /// <param name="value">The new value the needs to be assigned to the system variable</param>
        /// <returns></returns>
        /// <exception cref="HMApiException"><see cref="HMApiWrapper"/> is not initialized</exception>
        public bool SetVariable(HMBase hmElement, object value)
            => AsyncHelper.RunSync(() => SetStateAsync(hmElement, value));

        #endregion

        #region Homematic system messages

        /// <summary>
        /// Updates the global list of system messages
        /// </summary>
        /// <exception cref="HMApiException"><see cref="HMApiWrapper"/> is not initialized</exception>
        public void UpdateMessages()
            => AsyncHelper.RunSync(UpdateMessagesAsync);

        /// <summary>
        /// Confirms all messages and resets the global list of system messages
        /// </summary>
        /// <exception cref="HMApiException"><see cref="HMApiWrapper"/> is not initialized</exception>
        public void SetMessages()
            => AsyncHelper.RunSync(SetMessagesAsync);

        #endregion

        #region Homematic system states

        /// <summary>
        /// Triggers update of the global device list including their channels and data point or state data
        /// <param name="justQueryForFastUpdateDevices">Tells the method to just run an short update of an important set of devices</param>
        /// </summary>
        /// <exception cref="HMApiException"><see cref="HMApiWrapper"/> is not initialized</exception>
        public void UpdateStates(bool justQueryForFastUpdateDevices)
            => AsyncHelper.RunSync(() => UpdateStatesAsync(justQueryForFastUpdateDevices));

        /// <summary>
        /// Sets the state of a data point by address of the data point
        /// </summary>
        /// <param name="hmElement">An HMBase object that represents a channel or a data point</param>
        /// <param name="value">The new value the needs to be assigned to the data point</param>
        /// <returns>Result of operation; true if everything is okay</returns>
        /// <exception cref="HMApiException"><see cref="HMApiWrapper"/> is not initialized</exception>
        public bool SetState(HMBase hmElement, object value)
            => AsyncHelper.RunSync(() => SetStateAsync(hmElement, value));

        /// <summary>
        /// Sets the state of a data point by full address and key of the data point
        /// </summary>
        /// <param name="address">Full address of the desired channel</param>
        /// <param name="key">Key or typename of the desired data point</param>
        /// <param name="value">The new value the needs to be assigned to the data point</param>
        /// <returns>Result of operation; true if everything is okay</returns>
        /// <exception cref="HMApiException"><see cref="HMApiWrapper"/> is not initialized</exception>
        public bool SetStateByAddress(string address, string key, object value)
            => AsyncHelper.RunSync(() => SetStateByAddressAsync(address, key, value));

        #endregion

        #endregion
    }
}
