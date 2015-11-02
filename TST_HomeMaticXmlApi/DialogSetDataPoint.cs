using System;
using System.Windows.Forms;

namespace TRoschinsky.Lib.HomeMaticXmlApi
{
    public partial class DialogSetDataPoint : Form
    {
        public bool ValueWasSet = false;
        public string ValueToSet = String.Empty;
        public HMDeviceChannel DataChannel;

        public DialogSetDataPoint(HMDeviceChannel channel)
        {
            DataChannel = channel;
            InitializeComponent();
            Text += " " + channel.InternalId;
            lblDataPointName.Text = channel.ToString();
            txtValue.Text = String.Concat(channel.PrimaryValue);
        }

        private void btnSet_Click(object sender, EventArgs e)
        {
            ValueWasSet = true;
            ValueToSet = txtValue.Text;
            Close();
        }
    }
}
