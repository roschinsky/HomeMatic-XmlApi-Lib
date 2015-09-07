using System;
using System.Windows.Forms;

namespace TRoschinsky.Lib.HomeMaticXmlApi
{
    public partial class DialogSetDataPoint : Form
    {
        public bool ValueWasSet = false;
        public string ValueToSet = String.Empty;
        public int InternalIdToSet = 0;

        public DialogSetDataPoint(HMDeviceChannel channel)
        {
            InternalIdToSet = channel.PrimaryDataPoint.InternalId;
            InitializeComponent();
            Text += " " + InternalIdToSet;
            lblDataPointName.Text = channel.ToString();
            txtValue.Text = channel.PrimaryValue;
        }

        private void btnSet_Click(object sender, EventArgs e)
        {
            ValueWasSet = true;
            ValueToSet = txtValue.Text;
            Close();
        }
    }
}
