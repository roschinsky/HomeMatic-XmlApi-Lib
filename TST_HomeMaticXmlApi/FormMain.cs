using System;
using System.Drawing;
using System.Windows.Forms;

namespace TRoschinsky.Lib.HomeMaticXmlApi
{
    public partial class FormMain : Form
    {
        private HMApiWrapper hmc;

        public FormMain()
        {
            InitializeComponent();
        }

        private void txtConnect_Enter(object sender, EventArgs e)
        {
            if (txtConnect.ForeColor != SystemColors.WindowText)
            {
                txtConnect.ForeColor = SystemColors.WindowText;
                txtConnect.Text = String.Empty;
                txtConnect.AutoCompleteSource = AutoCompleteSource.AllUrl;
            }
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            try
            {
                Uri hmcUri = new Uri(txtConnect.Text);
                if (hmc == null)
                {
                    hmc = new HMApiWrapper(hmcUri);
                }

                if(hmc != null && hmc.Devices.Count > 0)
                {
                    treeView.Nodes.Clear();
                    TreeNode devNode = null;

                    foreach(HMDevice device in hmc.Devices)
                    {
                        devNode = new TreeNode(device.ToString());
                        foreach(HMDeviceChannel channel in device.Channels)
                        {
                            devNode.Nodes.Add(channel.ToString());
                        }

                        devNode.Collapse();
                        treeView.Nodes.Add(devNode);
                    }
                }
                else
                {
                    MessageBox.Show("Connect to HMC seems to be okay but there are no devices.", "No devices found...", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Connect to HMC failed...", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
