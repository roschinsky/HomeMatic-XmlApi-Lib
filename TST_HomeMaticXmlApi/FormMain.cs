using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace TRoschinsky.Lib.HomeMaticXmlApi
{
    public partial class FormMain : Form
    {
        private HMApiWrapper hmWrapper;
        private List<string> hmFastUpdates = new List<string>();
        public List<string> HmFastUpdates { get { return hmFastUpdates; } }

        public FormMain()
        {
            InitializeComponent();
            try
            {
                // Gets default URL to Homematic CCU2 and fill input field
                if (Properties.Settings.Default.UseDefaultUrl && !String.IsNullOrWhiteSpace(Properties.Settings.Default.HMDefaultUrl))
                {
                    txtConnect.Text = Properties.Settings.Default.HMDefaultUrl;
                    txtConnect_Enter(null, null);
                }

                // Gets all fast update devices or channels from config
                if (!String.IsNullOrWhiteSpace(Properties.Settings.Default.HMFastUpdate))
                {
                    string[] fastUpdateSettings = Properties.Settings.Default.HMFastUpdate.Trim().Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string fastUpdate in fastUpdateSettings)
                    {
                        hmFastUpdates.Add(fastUpdate.Trim());
                        listBoxFastUpdateDevices.Items.Add(fastUpdate.Trim());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Processing configuration failed...\n" + ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtConnect_Enter(object sender, EventArgs e)
        {
            if (txtConnect.ForeColor != SystemColors.WindowText)
            {
                txtConnect.ForeColor = SystemColors.WindowText;
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

                Properties.Settings.Default.HMDefaultUrl = txtConnect.Text.Trim();
                Properties.Settings.Default.Save();

                foreach (string fastUpdate in hmFastUpdates)
                {
                    hmWrapper.FastUpdateDeviceSetup(fastUpdate);
                }

                RefreshTreeView();

                string log = String.Empty;
                foreach(string logEntry in hmWrapper.Log)
                {
                    log += logEntry + Environment.NewLine;
                }

                MessageBox.Show(log);
            }
            catch (Exception ex)
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
                    hmWrapper.UpdateStates(checkBoxFastUpdateDevices.Checked);
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

            hmWrapper.UpdateStates(checkBoxFastUpdateDevices.Checked);

            if (hmWrapper != null && hmWrapper.Devices.Count > 0)
            {
                TreeNode devNode = new TreeNode("Devices");

                foreach (HMDevice device in hmWrapper.Devices)
                {
                    TreeNode devNodeParent = devNode.Nodes.Add(device.ToString());
                    devNodeParent.Tag = device;

                    foreach (HMDeviceChannel channel in device.Channels)
                    {
                        TreeNode channelNode = new TreeNode(channel.ToString());
                        channelNode.Tag = channel;

                        foreach (KeyValuePair<string, HMDeviceDataPoint> datapoint in channel.DataPoints)
                        {
                            TreeNode dataPointNode = new TreeNode(datapoint.Value.ToString());
                            dataPointNode.Tag = datapoint.Value;
                            channelNode.Nodes.Add(dataPointNode);
                        }

                        devNodeParent.Nodes.Add(channelNode);
                    }
                }

                treeView.Nodes.Add(devNode);
            }

            hmWrapper.UpdateVariables();

            if (hmWrapper != null && hmWrapper.Variables.Count > 0)
            {
                TreeNode varNodeParent = new TreeNode("Variables");

                foreach (HMSystemVariable variable in hmWrapper.Variables)
                {
                    TreeNode varNode = new TreeNode(variable.ToString());
                    varNode.Tag = variable;
                    varNodeParent.Nodes.Add(varNode);
                }

                treeView.Nodes.Add(varNodeParent);
            }

            hmWrapper.UpdateMessages();

            if (hmWrapper != null && hmWrapper.Messages.Count > 0)
            {
                TreeNode msgNodeParent = new TreeNode("Messages");

                foreach (HMSystemMessage message in hmWrapper.Messages)
                {
                    TreeNode msgNode = new TreeNode(message.ToString());
                    msgNode.Tag = message;
                    msgNodeParent.Nodes.Add(msgNode);
                }

                treeView.Nodes.Add(msgNodeParent);
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
                HMBase hmElement = treeView.SelectedNode.Tag as HMBase;
                if (hmElement != null)
                {
                    DialogHMData dataPointDialog = new DialogHMData(hmElement);
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
                if (e.Cancel)
                {
                    return;
                }
                else
                {
                    DialogHMData dataPointDialog = sender as DialogHMData;
                    if (dataPointDialog != null && dataPointDialog.ValueWasSet && dataPointDialog.HMElement != null)
                    {
                        bool success = false;
                        if (dataPointDialog.HMElement.GetType() == typeof(HMSystemVariable))
                        {
                            success = hmWrapper.SetVariable(dataPointDialog.HMElement, dataPointDialog.ValueToSet);
                        }
                        else if (dataPointDialog.HMElement.GetType() == typeof(HMDeviceChannel) ||
                            dataPointDialog.HMElement.GetType() == typeof(HMDeviceDataPoint))
                        {
                            success = hmWrapper.SetState(dataPointDialog.HMElement, dataPointDialog.ValueToSet);
                        }
                        else
                        {
                            MessageBox.Show(String.Format("Was not able to operate a type of {0}", dataPointDialog.HMElement.GetType().Name),
                                "Type to set unknown...", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                            return;
                        }

                        MessageBox.Show(String.Format("Operation result: {0}", success ? "SUCCESS" : "FAILED"),
                            String.Format("Setting {0}...", dataPointDialog.HMElement.GetType().Name),
                            MessageBoxButtons.OK, success ? MessageBoxIcon.Information : MessageBoxIcon.Error);

                        RefreshTreeView();
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
                if (hmWrapper != null)
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
            if (e.KeyCode == Keys.Enter)
            {
                btnConnect_Click(sender, null);
            }
        }
    }
}
