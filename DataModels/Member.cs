using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApplication1.DataModels
{
    public class Member
    {
        public int MemberID = -1;
        public string FirstName = null;
        public string LastName = null;
        public string PhoneNum = null;
        public Tag ActiveTag = null;
        public int TotalPoints = 0;

        public Member()
        {
        }

        public Member(DataTable dt)
        {
            if (dt.Rows.Count == 1)
            {
                var row = (DataRow)dt.Rows[0];

                FirstName = (string)row["FirstName"];
                LastName = (string)row["LastName"];
                PhoneNum = (string)row["Phone"];

                if (!(row["CardID"] is DBNull))
                {
                    ActiveTag = new Tag();
                    ActiveTag.TagNum = (string)row["CardID"];
                    ActiveTag.Info = (string)row["CardType"];
                    ActiveTag.LastActivity = (DateTime)row["CardLastSeen"];
                }

                TotalPoints = row["TotalPoints"] is DBNull ? 0 : (int)row["TotalPoints"];

                row = null;
            }
        }

    }
}
