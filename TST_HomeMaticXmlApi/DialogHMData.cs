using System;
using System.Windows.Forms;

namespace TRoschinsky.Lib.HomeMaticXmlApi
{
    public partial class DialogHMData : Form
    {
        public bool ValueWasSet = false;
        public string ValueToSet = String.Empty;
        public HMBase HMElement;

        public DialogHMData(HMBase hmElement)
        {
            HMElement = hmElement;
            InitializeComponent();

            try
            {
                Text += HMElement.GetType().Name;
                lblDataPointName.Text = hmElement.ToString();

                switch (Text)
                {
                    case "HMDevice":
                        txtValue.ReadOnly = true;
                        txtValue.Text = "A device can not be set directly; choose a channel or data point!";
                        break;

                    case "HMDeviceChannel":
                        HMDeviceChannel chan = (HMDeviceChannel)HMElement;
                        txtValue.Text = String.Concat(chan.PrimaryValue);
                        btnSet.Enabled = true;
                        break;

                    case "HMDeviceDataPoint":
                        HMDeviceDataPoint dp = (HMDeviceDataPoint)HMElement;
                        txtValue.Text = String.Concat(dp.ValueString);
                        btnSet.Enabled = true;
                        break;

                    case "HMSystemMessage":
                        txtValue.ReadOnly = true;
                        txtValue.Text = "A message can not be set directly; choose a channel or data point!";
                        break;

                    case "HMSystemVariable":
                        HMSystemVariable svar = (HMSystemVariable)HMElement;
                        txtValue.Text = String.Concat(svar.Value);
                        btnSet.Enabled = true;
                        break;

                    default:
                        txtValue.ReadOnly = true;
                        txtValue.Text = "Don't know what it is; choose a channel or data point!";
                        break;

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Identifing Homematic element failed...\n" + ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSet_Click(object sender, EventArgs e)
        {
            ValueWasSet = true;
            ValueToSet = txtValue.Text;
            Close();
        }
    }
}
