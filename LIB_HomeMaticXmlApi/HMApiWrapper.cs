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
        private string xmlApiMethodVariable = "sysvarlist";

        private Uri HMUrl;

        private List<HMDevice> devices = new List<HMDevice>();
        public List<HMDevice> Devices { get { return devices;} }


        public HMApiWrapper(Uri homeMaticUri)
        {
            HMUrl = homeMaticUri;
            Initialize(false);
        }

        public HMApiWrapper(Uri homeMaticUri, bool initializeWithStates)
        {
            HMUrl = homeMaticUri;
            Initialize(initializeWithStates);
        }


        private void Initialize(bool fullInit)
        {
            devices = GetDevices();
            
            if(fullInit)
            {
                GetStates();
            }
        }

        private void GetStates()
        {
            XmlDocument xmlStates = GetApiData(xmlApiMethodStateAll);

            foreach (XmlElement devElement in xmlStates.DocumentElement.ChildNodes)
            {
                int devIseId = int.Parse(devElement.GetAttribute("ise_id"));
                HMDevice device = devices.First(d => devIseId == d.InternalId);

                foreach (XmlElement chanElement in devElement.ChildNodes)
                {
                    int chanIseId = int.Parse(devElement.GetAttribute("ise_id"));
                    HMDeviceChannel channel = device.Channels.First(c => chanIseId == c.InternalId);
                    channel.Value = chanElement.GetAttribute("ise_id");
                }
            }
        }

        private List<HMDevice> GetDevices()
        {
            List<HMDevice> result = new List<HMDevice>();

            XmlDocument xmlDevices = GetApiData(xmlApiMethodDevice);

            foreach(XmlElement devElement in xmlDevices.DocumentElement.ChildNodes)
            {
                HMDevice device = new HMDevice()
                {
                    Name = devElement.GetAttribute("name"),
                    Address = devElement.GetAttribute("address"),
                    InternalId = int.Parse(devElement.GetAttribute("ise_id")),
                    DeviceType = devElement.GetAttribute("device_type")
                };

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
    }
}
