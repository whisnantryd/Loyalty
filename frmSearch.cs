using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using Loyalty.DataAccess;
using Loyalty.DataModels;

namespace Loyalty
{
    public partial class frmSearch : Form
    {

        MemberTagIO MTO;

        Member selectedMem = null;
        public Member SelectedMember
        {
            get
            {
                return selectedMem;
            }
        }

        public frmSearch(MemberTagIO mto)
        {
            InitializeComponent();

            MTO = mto;
        }

        #region FormInput

        private void searchBtn_Click(object sender, EventArgs e)
        {
            List<MemberFilter> filters = new List<MemberFilter>();

            if (txtName.Text != "")
            {
                filters.Add(new MemberFilter()
                {
                    data = txtName.Text,
                    filtertype = MemberFilterTypes.Name
                });
            }

            if (txtPhone.Text != "")
            {
                filters.Add(new MemberFilter()
                {
                    data = txtPhone.Text,
                    filtertype = MemberFilterTypes.Phone
                });
            }

            MTO.GetMember(filters, getMember_Callback);
        }

        private void lstSearchResults_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void okBtn_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void cancelBtn_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        #endregion

        #region Callbacks

        void getMember_Callback(SQLDataRequest sdr, DataTable dt)
        {
            if (this.InvokeRequired)
            {
                //var m = new RequestCallbackHandler(getMember_Callback);
                //this.Invoke(m, new object[2] { sdr, dt });
                //return;

                this.BeginInvoke(new MethodInvoker(() => getMember_Callback(sdr, dt)));
                return;
            }

            lstSearchResults.Items.Clear();

            Member mem = null;

            foreach (DataRow row in dt.Rows)
            {
                mem = new Member(row);
                lstSearchResults.Items.Add(new ListViewItem(new string[] { mem.FirstName + " " + mem.LastName, mem.PhoneNum, mem.TotalPoints.ToString() }, 0));
            }

            mem = null;
        }

        #endregion

    }
}
