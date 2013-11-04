using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Phidgets;
using Phidgets.Events;
using WindowsFormsApplication1.DataAccess;
using WindowsFormsApplication1.DataModels;

namespace WindowsFormsApplication1
{
    public partial class frmMain : Form
    {
        private MemberTagIO MTO;
        private RFID rfid;

        public frmMain()
        {
            InitializeComponent();

            MTO = new MemberTagIO();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            rfid = new RFID();

            rfid.Attach += new AttachEventHandler(rfid_Attach);
            rfid.Detach += new DetachEventHandler(rfid_Detach);
            rfid.Error += new ErrorEventHandler(rfid_Error);

            rfid.Tag += new TagEventHandler(rfid_Tag);
            rfid.TagLost += new TagEventHandler(rfid_TagLost);

            rfid.open(-1);
        }

        // called when a new rfid reader is attached to the usb port
        void rfid_Attach(object sender, AttachEventArgs e)
        {
            RFID attached = (RFID)sender;

            if (rfid.outputs.Count > 0)
            {
                rfid.Antenna = true;
            }

            //MessageBox.Show("Connected to loyalty card reader");
        }

        // called when an attached rfid reader is unplugged or powered down
        void rfid_Detach(object sender, DetachEventArgs e)
        {
            RFID detached = (RFID)sender;

            if (rfid.outputs.Count > 0)
            {

            }

            MessageBox.Show("Lost connection to loyalty card reader!");
        }

        void rfid_Error(object sender, ErrorEventArgs e)
        {
            Phidget p = (Phidget)sender;
        }

        // called when an attached rfid reader detects a tag
        void rfid_Tag(object sender, TagEventArgs e)
        {
            if (this.InvokeRequired)
            {
                var m = new TagEventHandler(rfid_Tag);
                this.Invoke(m, new object[2] { sender, e });

                return;
            }

            // Update last activity for this tag
            MTO.UpdateTag(new Tag
            {
                TagNum = e.Tag,
                LastActivity = DateTime.Now
            });


            // Show the operator form
            var frm = new frmOperator(MTO);
            frm.Show();
            frm.SetWorkingCard(e.Tag);

            //// for debugging
            //var lvi = new ListViewItem();
            //lvi.Text = "Tag";
            //lvi.SubItems.Add(DateTime.Now.ToLongTimeString());
            //lvi.SubItems.Add("Observed tag '" + e.Tag + "'");

            //listView1.Items.Add(lvi);
        }

        // called when an attached rfid reader loses detection of a tag
        void rfid_TagLost(object sender, TagEventArgs e)
        {
            if (this.InvokeRequired)
            {
                var m = new TagEventHandler(rfid_TagLost);
                this.Invoke(m, new object[2] { sender, e });

                return;
            }

            WindowState = FormWindowState.Minimized;
        }

        // when the application is being terminated, close the Phidget.
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            rfid.Attach -= new AttachEventHandler(rfid_Attach);
            rfid.Detach -= new DetachEventHandler(rfid_Detach);
            rfid.Tag -= new TagEventHandler(rfid_Tag);
            rfid.TagLost -= new TagEventHandler(rfid_TagLost);

            //run any events in the message queue - otherwise close will hang if there are any outstanding events
            Application.DoEvents();

            rfid.close();
        }

    }
}
