using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace TRoschinsky.Lib.HomeMaticXmlApi
{
    public partial class FormMain : Form
    {
        private HMApiWrapper hmWrapper;

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
                if (hmWrapper == null)
                {
                    hmWrapper = new HMApiWrapper(new Uri(txtConnect.Text), false, false);
                }

                hmWrapper.FastUpdateDeviceSetup("LEQ0502263:1");
                hmWrapper.FastUpdateDeviceSetup("LEQ0412714");
                hmWrapper.FastUpdateDeviceSetup("LEQ1468091:1");

                RefreshTreeView();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Connect to HMC failed...\n" + ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnRequestHighPrioUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                if (hmWrapper != null)
                {
                    hmWrapper.UpdateStates(hmWrapper.FastUpdateDevices.Count > 0);
                    RefreshTreeView();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "High-Prio update failed...\n" + ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RefreshTreeView()
        {
            treeView.Nodes.Clear();

            hmWrapper.UpdateStates(false);

            if (hmWrapper != null && hmWrapper.Devices.Count > 0)
            {
                TreeNode devNode = new TreeNode("Devices");

                foreach (HMDevice device in hmWrapper.Devices)
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

            hmWrapper.UpdateVariables();

            if (hmWrapper != null && hmWrapper.Variables.Count > 0)
            {
                TreeNode varNode = new TreeNode("Variables");

                foreach (HMSystemVariable variable in hmWrapper.Variables)
                {
                    varNode.Nodes.Add(variable.ToString());
                }

                treeView.Nodes.Add(varNode);
            }

            hmWrapper.UpdateMessages();

            if (hmWrapper != null && hmWrapper.Messages.Count > 0)
            {
                TreeNode msgNode = new TreeNode("Messages");

                foreach (HMSystemMessage message in hmWrapper.Messages)
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
                MessageBox.Show(ex.Message, "Error obtaining channel...\n" + ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                        if(hmWrapper.SetState(dataPointDialog.DataChannel, dataPointDialog.ValueToSet))
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

        private void btnMessagesClear_Click(object sender, EventArgs e)
        {
            try
            {
                if(hmWrapper != null)
                {
                    hmWrapper.SetMessages();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Clearing messages failed...\n" + ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
