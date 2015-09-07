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
                    hmc = new HMApiWrapper(hmcUri, false, false);
                }

                hmc.FastUpdateDeviceSetup("LEQ0502263:1");
                hmc.FastUpdateDeviceSetup("LEQ0412714");
                hmc.FastUpdateDeviceSetup("LEQ1468091:1");

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

            hmc.UpdateStates(false);

            if (hmc != null && hmc.Devices.Count > 0)
            {
                TreeNode devNode = new TreeNode("Devices");

                foreach (HMDevice device in hmc.Devices)
                {
                    TreeNode devNodeSub = devNode.Nodes.Add(device.ToString());
                    foreach (HMDeviceChannel channel in device.Channels)
                    {
                        TreeNode channelNode = new TreeNode(channel.ToString());
                        channelNode.Tag = channel;
                        devNodeSub.Nodes.Add(channelNode);
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

        private void treeView_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                HMDeviceChannel channel = treeView.SelectedNode.Tag as HMDeviceChannel;
                if(channel != null)
                {
                    DialogSetDataPoint dataPointDialog = new DialogSetDataPoint(channel);
                    dataPointDialog.FormClosing += DataPointDialog_FormClosing;
                    dataPointDialog.ShowDialog();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error obtaining channel...", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DataPointDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if(e.Cancel)
                {
                    return;
                }
                else
                {
                    DialogSetDataPoint dataPointDialog = sender as DialogSetDataPoint;
                    if(dataPointDialog != null && dataPointDialog.ValueWasSet)
                    {
                        if(hmc.SetState(dataPointDialog.InternalIdToSet, dataPointDialog.ValueToSet))
                        {
                            MessageBox.Show("Operation result: SUCCESS", "Operating channel...", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("Operation result: FAILURE", "Operating channel...", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error operating channel...", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
