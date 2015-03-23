using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using System.Xml;

namespace TRoschinsky.Lib.HomeMaticXmlApi
{
    public class HMApiWrapper
    {
        private string xmlApiDefaultPath = "/addons/xmlapi";

        private string xmlApiMethodDevice = "devicelist";
        private string xmlApiMethodStatusAll = "statelist";
        private string xmlApiMethodStatusSingle = "state";

        private Uri HMUrl;

        private List<HMDevice> devices = new List<HMDevice>();
        public List<HMDevice> Devices { get { return devices;} }


        public HMApiWrapper(Uri homeMaticUri)
        {
            HMUrl = homeMaticUri;
            Initialize();
        }

        private void Initialize()
        {
            devices = GetDevices();
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

            foreach(HMDevice device in devices)
            {
                if(address.Equals(device.Address))
                {
                    result = device.InternalId;
                    break;
                }
                
                if(searchChannels)
                {
                    foreach(HMDeviceChannel channel in device.Channels)
                    {
                        if (address.Equals(channel.Address))
                        {
                            result = device.InternalId;
                            break;
                        }
                    }
                }
            }

            return result;
        }
    }
}
