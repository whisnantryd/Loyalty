using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loyalty.DataModels
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

        public Member(DataRow row)
        {
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
        }

    }
}
