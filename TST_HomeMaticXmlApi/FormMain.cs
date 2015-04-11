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
                if (hmc == null)
                {
                    Uri hmcUri = new Uri(txtConnect.Text);
                    hmc = new HMApiWrapper(hmcUri, true, false);
                }

                hmc.SetupHighPrioDevice("LEQ0501253:1");
                hmc.SetupHighPrioDevice("LEQ0422834");
                hmc.SetupHighPrioDevice("LEQ1447097:2");

                RefreshTreeView();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Connect to HMC failed...", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnRequestHighPrioUpdate_Click(object sender, EventArgs e)
        {
            if (hmc != null)
            {
                hmc.UpdateStates(true);
                RefreshTreeView();
            }
        }

        private void RefreshTreeView()
        {
            treeView.Nodes.Clear();

            if (hmc != null && hmc.Devices.Count > 0)
            {
                TreeNode devNode = new TreeNode("Devices");

                foreach (HMDevice device in hmc.Devices)
                {
                    TreeNode devNodeSub = devNode.Nodes.Add(device.ToString());
                    foreach (HMDeviceChannel channel in device.Channels)
                    {
                        devNodeSub.Nodes.Add(channel.ToString());
                    }
                }

                treeView.Nodes.Add(devNode);
            }

            hmc.UpdateVariables();

            if (hmc != null && hmc.Variables.Count > 0)
            {
                TreeNode varNode = new TreeNode("Variables");

                foreach (HMSystemVariable variable in hmc.Variables)
                {
                    varNode.Nodes.Add(variable.ToString());
                }

                treeView.Nodes.Add(varNode);
            }

            hmc.UpdateMessages();

            if (hmc != null && hmc.Messages.Count > 0)
            {
                TreeNode msgNode = new TreeNode("Messages");

                foreach (HMSystemMessage message in hmc.Messages)
                {
                    msgNode.Nodes.Add(message.ToString());
                }

                treeView.Nodes.Add(msgNode);
            }

            if (treeView.Nodes.Count == 0)
            {
                MessageBox.Show("Connect to HMC seems to be okay but there are no devices.", "No devices found...", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
