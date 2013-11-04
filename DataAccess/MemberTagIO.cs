using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsFormsApplication1.DataModels;

namespace WindowsFormsApplication1.DataAccess
{
    public class MemberTagIO
    {

        public SQLDataCollector DataCollector = new SQLDataCollector("localhost\\SQLEXPRESS", "Loyalty", "abbeyUser", "beer");

        public void GetMember(string filterdata, MemberFilter filtertype, RequestCallbackHandler callback)
        {
            var cmd = DataCollector.BuildNewSQLCommand("GetMember");

            switch (filtertype)
            {
                case MemberFilter.MemberID:
                    cmd.Parameters.AddWithValue("PkMemberID", filterdata);
                    break;
                case MemberFilter.Name:
                    cmd.Parameters.AddWithValue("Name", filterdata);
                    break;
                case MemberFilter.Phone:
                    cmd.Parameters.AddWithValue("Phone", filterdata);
                    break;
                case MemberFilter.Tag:
                    cmd.Parameters.AddWithValue("Tag", filterdata);
                    break;
            }

            DataCollector.QueueRequest(new SQLDataRequest(cmd, callback));

            cmd = null;
            filterdata = null;
        }

        public void UpdateMember(Member memberdata, RequestCallbackHandler callback)
        {
            // Check if a key is supplied
            if (-1 == memberdata.MemberID)
                throw new NullKeyException();

            var cmd = DataCollector.BuildNewSQLCommand("UpdateMember");

            // Add required parameters
            cmd.Parameters.AddWithValue("MemberID", memberdata.MemberID);

            // Add optional parameters
            if (null != memberdata.FirstName)
                cmd.Parameters.AddWithValue("FirstName", memberdata.FirstName);

            if (null != memberdata.LastName)
                cmd.Parameters.AddWithValue("LastName", memberdata.LastName);

            if (null != memberdata.PhoneNum)
                cmd.Parameters.AddWithValue("PhoneNum", memberdata.PhoneNum);

            if (null != memberdata.ActiveTag)
                cmd.Parameters.AddWithValue("TagNum", memberdata.ActiveTag.TagNum);

            DataCollector.QueueRequest(new SQLDataRequest(cmd, callback));

            cmd = null;
        }

        public void UpdateTag(Tag tagdata)
        {
            // Check if a key is supplied
            if (null == tagdata.TagNum)
                throw new NullKeyException();

            var cmd = DataCollector.BuildNewSQLCommand("UpdateTag");

            // Add Required parameters
            if (null != tagdata.TagNum)
                cmd.Parameters.AddWithValue("TagID", tagdata.TagNum);

            // Add optional parameters
            if (null != tagdata.Info)
                cmd.Parameters.AddWithValue("Info", tagdata.Info);

            if (DateTime.MinValue != tagdata.LastActivity)
                cmd.Parameters.AddWithValue("LastActivity", DataCollector.GetSQLDateTime(tagdata.LastActivity));

            DataCollector.QueueRequest(new SQLDataRequest(cmd));

            cmd = null;
        }

        public void AddMemberPoints(Member memberdata, int points)
        {
            // Check if a key is supplied
            if (-1 == memberdata.MemberID)
                throw new NullKeyException();

            var cmd = DataCollector.BuildNewSQLCommand("InsertMemberPoints");

            // Add required parameters
            cmd.Parameters.AddWithValue("MemberID", memberdata.MemberID);
            cmd.Parameters.AddWithValue("Points", points);

            // Add optional parameters
            if (null != memberdata.ActiveTag)
                cmd.Parameters.AddWithValue("TagNum", memberdata.ActiveTag.TagNum);

            DataCollector.QueueRequest(new SQLDataRequest(cmd));

            cmd = null;
        }

    }

    public enum MemberFilter
    {
        MemberID,
        Tag,
        Name,
        Phone
    }

    public class NullKeyException : Exception
    {

        public override string Message
        {
            get
            {
                return "Missing object key. An update cannot be performed on an object without the key.";
            }
        }

    }

}
