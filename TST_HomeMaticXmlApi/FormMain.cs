using System;
using System.Collections.Generic;
using System.Diagnostics;
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
                    txtConnect.ForeColor = SystemColors.WindowText;
                }
                else
                {
                    txtConnect.Text = Properties.Resources.txtConnectDefault;
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
                MessageBox.Show(ex.Message, "Processing configuration failed...", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            Stopwatch sw = new Stopwatch();

            try
            {                
                sw.Start();
                hmWrapper = new HMApiWrapper(new Uri(txtConnect.Text), false, false);
                sw.Stop();

                Properties.Settings.Default.HMDefaultUrl = txtConnect.Text.Trim();
                Properties.Settings.Default.Save();

                RefreshTreeView();
                EnableButtonsWhenConnected();

                WriteStatus("Connected! Got all the devices - now click the Update button!",
                    String.Format("*** {0}: Connected to Homematic CCU2 at {1} ***", DateTime.Now, hmWrapper.HmUrl),
                    sw.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                WriteStatus("Connect failed!",
                    String.Format("*** {0}: Connect to Homematic CCU2 at {1} failed ***", DateTime.Now, txtConnect.Text),
                    sw.ElapsedMilliseconds);
                MessageBox.Show(ex.Message, "Connect to HMC failed...", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnUpdateRequest_Click(object sender, EventArgs e)
        {
            try
            {
                if (hmWrapper != null)
                {
                    // Setup wrapper with items from fast update list
                    foreach (string fastUpdate in hmFastUpdates)
                    {
                        hmWrapper.FastUpdateDeviceSetup(fastUpdate);
                    }

                    Stopwatch sw = new Stopwatch();
                    sw.Start();
                    hmWrapper.UpdateStates(checkBoxFastUpdateDevices.Checked);
                    sw.Stop();

                    WriteStatus(String.Format("Update done {0}", checkBoxFastUpdateDevices.Checked ? "fast" : "full"),
                        String.Format("- UPDATE({0}) @ {1}: {2}ms", checkBoxFastUpdateDevices.Checked ? "fast" : "full", DateTime.Now, sw.ElapsedMilliseconds),
                        sw.ElapsedMilliseconds);

                    RefreshTreeView();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Update failed...", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RefreshTreeView()
        {
            treeView.Nodes.Clear();

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

                devNode.Expand();
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

                varNodeParent.Expand();
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

                msgNodeParent.Expand();
                treeView.Nodes.Add(msgNodeParent);
            }

            if (treeView.Nodes.Count == 0)
            {
                MessageBox.Show("Connect to HMC seems to be okay but there are no devices.", "No devices found...", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            foreach (string logEntry in hmWrapper.Log)
            {
                richTextBoxLog.AppendText(logEntry);
            }
        }

        private void treeView_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                if (treeView.SelectedNode != null && treeView.SelectedNode.Tag != null)
                {
                    HMBase hmElement = treeView.SelectedNode.Tag as HMBase;
                    if (hmElement != null)
                    {
                        DialogHMData dataPointDialog = new DialogHMData(hmElement);
                        dataPointDialog.FormClosing += DataPointDialog_FormClosing;
                        dataPointDialog.ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error obtaining Homematic element...", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    Stopwatch sw = new Stopwatch();
                    if (dataPointDialog != null && dataPointDialog.ValueWasSet && dataPointDialog.HMElement != null)
                    {
                        bool wasSuccessful = false;
                        if (dataPointDialog.HMElement.GetType() == typeof(HMSystemVariable))
                        {
                            sw.Start();
                            wasSuccessful = hmWrapper.SetVariable(dataPointDialog.HMElement, dataPointDialog.ValueToSet);
                            sw.Stop();
                        }
                        else if (dataPointDialog.HMElement.GetType() == typeof(HMDeviceChannel) ||
                            dataPointDialog.HMElement.GetType() == typeof(HMDeviceDataPoint))
                        {
                            sw.Start();
                            wasSuccessful = hmWrapper.SetState(dataPointDialog.HMElement, dataPointDialog.ValueToSet);
                            sw.Stop();
                        }
                        else
                        {
                            WriteStatus(String.Format("Was not able to operate a type of {0}", dataPointDialog.HMElement.GetType().Name), null, 0);
                            return;
                        }

                        WriteStatus(String.Format("Operation of {0} {1}", dataPointDialog.HMElement.GetType().Name, wasSuccessful ? "successful" : "failed"),
                            String.Format("- OPERATE({0}) @ {1}: Executed in {2}ms", dataPointDialog.HMElement.Address, DateTime.Now, sw.ElapsedMilliseconds),
                           sw.ElapsedMilliseconds);
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
                MessageBox.Show(ex.Message, "Clearing messages failed...", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void linkLabelGitHub_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try { Process.Start(linkLabelGitHub.Text); }
            catch { }
        }

        private void txtConnect_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnConnect_Click(sender, null);
            }
        }
        private void txtConnect_Enter(object sender, EventArgs e)
        {
            if (txtConnect.ForeColor != SystemColors.WindowText)
            {
                txtConnect.ForeColor = SystemColors.WindowText;
                txtConnect.AutoCompleteSource = AutoCompleteSource.AllUrl;
                txtConnect.Text = String.Empty;
            }
            btnConnect.Enabled = true;
        }

        private void txtConnect_Leave(object sender, EventArgs e)
        {
            if(String.IsNullOrWhiteSpace(txtConnect.Text))
            {
                if (Properties.Settings.Default.UseDefaultUrl && !String.IsNullOrWhiteSpace(Properties.Settings.Default.HMDefaultUrl))
                {
                    txtConnect.Text = Properties.Settings.Default.HMDefaultUrl;
                }
                else
                {
                    txtConnect.ForeColor = SystemColors.ControlDark;
                    txtConnect.Text = Properties.Resources.txtConnectDefault;
                    btnConnect.Enabled = false;
                }
            }
        }

        private void WriteStatus(string statusMessage, string logMessage, long executionTime)
        {
            if(!String.IsNullOrWhiteSpace(statusMessage))
            {
                toolStripStatusLabel1.Text = statusMessage;
            }

            if(!String.IsNullOrWhiteSpace(logMessage))
            {
                richTextBoxLog.AppendText(logMessage + Environment.NewLine);
            }

            if(executionTime > 0)
            {
                toolStripStatusLabel2.Text = String.Format("Last execution in {0}ms", executionTime);
            }
        }

        private void btnAddFastUpdateDevice_Click(object sender, EventArgs e)
        {
            if(treeView.SelectedNode != null && treeView.SelectedNode.Tag != null)
            {
                HMBase hmElement = treeView.SelectedNode.Tag as HMBase;
                if(hmElement != null && !String.IsNullOrWhiteSpace(hmElement.Address) && !hmFastUpdates.Contains(hmElement.Address))
                {
                    listBoxFastUpdateDevices.Items.Add(hmElement.Address.Trim());
                    hmFastUpdates.Add(hmElement.Address.Trim());
                }
            }
        }

        private void EnableButtonsWhenConnected()
        {
            btnAddFastUpdateDevice.Enabled = true;
            btnMessagesClear.Enabled = true;
            btnUpdateRequest.Enabled = true;
            checkBoxFastUpdateDevices.Enabled = true;
        }
    }
}
