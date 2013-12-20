using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loyalty.DataModels
{
    public class Tag
    {
        public string TagNum = null;
        public string Info = null;
        public DateTime LastActivity = DateTime.MinValue;

        public Tag()
        {
        }

        public Tag(DataTable dt)
        {
            if (dt.Rows.Count == 1)
            {
                var row = (DataRow)dt.Rows[0];

                TagNum = (string)row["ID"];
                Info = (string)row["Info"];
                LastActivity = (DateTime)row["LastActivity"];

                row = null;
            }
        }

    }
}
