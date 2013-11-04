using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsApplication1.DataAccess;

namespace WindowsFormsApplication1
{
    public partial class frmOperator : Form
    {
        string workingCardID = "";

        MemberTagIO memTagIO;

        public frmOperator(MemberTagIO MTO)
        {
            // Required
            InitializeComponent();

            memTagIO = MTO;

            // Set event handlers
            foreach (Control obj in groupBox2.Controls)
            {
                if (obj.GetType() == typeof(Button))
                {
                    var btn = (Button)obj;
                    int i = 0;

                    if (int.TryParse(btn.Text, out i))
                    {
                        btn.Click += numberButtonPress;
                    }
                }
            }
        }

        public void SetWorkingCard(string cardUID)
        {
            workingCardID = cardUID;

            memTagIO.GetMember(cardUID, MemberFilter.Tag, getMember_Callback);
        }

        #region Form_Input
        
        private void clrBtn_Click(object sender, EventArgs e)
        {
            txtAddAmount.Text = "";
        }

        void numberButtonPress(object sender, EventArgs e)
        {
            var btn = (Button)sender;

            txtAddAmount.Text = txtAddAmount.Text + btn.Text;

            btn = null;

            txtAddAmount.Text = txtAddAmount.Text.TrimStart('0');
        }

        #endregion

        #region Database_Callbacks

        void getTag_Callback(SQLDataRequest sdr, DataTable dt)
        {
            if (this.InvokeRequired)
            {
                var m = new RequestCallbackHandler(getTag_Callback);
                this.Invoke(m, new object[2] { sdr, dt });
                return;
            }

            if (dt.Rows.Count == 1)
            {
                var row = (DataRow)dt.Rows[0];
            }
        }

        void getMember_Callback(SQLDataRequest sdr, DataTable dt)
        {
            if (this.InvokeRequired)
            {
                var m = new RequestCallbackHandler(getMember_Callback);
                this.Invoke(m, new object[2] { sdr, dt });
                return;
            }

            if (dt.Rows.Count == 1)
            {
                var row = (DataRow)dt.Rows[0];

                txtFirstName.Text = (string)row["FirstName"];
                txtLastName.Text = (string)row["LastName"];
                txtPhone.Text = (string)row["Phone"];
                txtTotalPoints.Text = row["TotalPoints"] is DBNull ? "0" : (string)row["TotalPoints"];

                recentTrans.Items.Clear();

                row = null;
            }
        }

        #endregion

    }
}
