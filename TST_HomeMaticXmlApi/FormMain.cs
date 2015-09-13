using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace TRoschinsky.Lib.HomeMaticXmlApi
{
    public partial class FormMain : Form
    {
        private HMApiWrapper hmcProxy;

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
                if (hmcProxy == null)
                {
                    hmcProxy = new HMApiWrapper(new Uri(txtConnect.Text), false, false);
                }

                hmcProxy.FastUpdateDeviceSetup("LEQ0502263:1");
                hmcProxy.FastUpdateDeviceSetup("LEQ0412714");
                hmcProxy.FastUpdateDeviceSetup("LEQ1468091:1");

                RefreshTreeView();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Connect to HMC failed...", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnRequestHighPrioUpdate_Click(object sender, EventArgs e)
        {
            if (hmcProxy != null)
            {
                hmcProxy.UpdateStates(true);
                RefreshTreeView();
            }
        }

        private void RefreshTreeView()
        {
            treeView.Nodes.Clear();

            hmcProxy.UpdateStates(false);

            if (hmcProxy != null && hmcProxy.Devices.Count > 0)
            {
                TreeNode devNode = new TreeNode("Devices");

                foreach (HMDevice device in hmcProxy.Devices)
                {
                    TreeNode devNodeSub = devNode.Nodes.Add(device.ToString());
                    foreach (HMDeviceChannel channel in device.Channels)
                    {
                        TreeNode channelNode = new TreeNode(channel.ToString());
                        channelNode.Tag = channel;

                        foreach(KeyValuePair<string, HMDeviceDataPoint> datapoint in channel.DataPoints)
                        {
                            channelNode.Nodes.Add(datapoint.Value.ToString());
                        }

                        devNodeSub.Nodes.Add(channelNode);
                    }
                }

                treeView.Nodes.Add(devNode);
            }

            hmcProxy.UpdateVariables();

            if (hmcProxy != null && hmcProxy.Variables.Count > 0)
            {
                TreeNode varNode = new TreeNode("Variables");

                foreach (HMSystemVariable variable in hmcProxy.Variables)
                {
                    varNode.Nodes.Add(variable.ToString());
                }

                treeView.Nodes.Add(varNode);
            }

            hmcProxy.UpdateMessages();

            if (hmcProxy != null && hmcProxy.Messages.Count > 0)
            {
                TreeNode msgNode = new TreeNode("Messages");

                foreach (HMSystemMessage message in hmcProxy.Messages)
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
                        if(hmcProxy.SetState(dataPointDialog.InternalIdToSet, dataPointDialog.ValueToSet))
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

        private void linkLabelGitHub_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try { System.Diagnostics.Process.Start(linkLabelGitHub.Text); }
            catch { }
        }

        private void txtConnect_KeyUp(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                btnConnect_Click(sender, null);
            }
        }
    }
}
