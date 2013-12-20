using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Timers;

namespace Loyalty.DataAccess
{

    public class SQLDataCollector
    {

        public SqlConnectionStringBuilder DatabaseConnection = new SqlConnectionStringBuilder();

        Queue<SQLDataRequest> requestQueue = new Queue<SQLDataRequest>();

        BackgroundWorker bgwRunQueue = null;

        public SQLDataCollector()
        {
            bgwRunQueue = new BackgroundWorker();
            bgwRunQueue.DoWork += processQueue;
        }

        public SQLDataCollector(string datasource, string initialcatalog, string user, string password)
        {
            DatabaseConnection.DataSource = datasource;
            DatabaseConnection.InitialCatalog = initialcatalog;
            DatabaseConnection.UserID = user;
            DatabaseConnection.Password = password;
            DatabaseConnection.ConnectTimeout = 7;

            bgwRunQueue = new BackgroundWorker();
            bgwRunQueue.DoWork += processQueue;
        }

        public SqlCommand BuildNewSQLCommand(string procedurename)
        {
            var cmd = new SqlCommand(procedurename);
            cmd.CommandType = CommandType.StoredProcedure;

            return cmd;
        }

        public void QueueRequest(SQLDataRequest request)
        {
            var isQueued = (bool)requestQueue.Contains(request);

            // Add request to the queue if not already queued
            if (!isQueued)
                requestQueue.Enqueue(request);

            // Fire the queue process
            if (!bgwRunQueue.IsBusy)
                bgwRunQueue.RunWorkerAsync();
        }

        void processQueue(object sender, EventArgs e)
        {
            if (requestQueue.Count == 0)
                return;

            // Initialize and open the SQL server connection
            var sqlcon = new SqlConnection();
            sqlcon.ConnectionString = DatabaseConnection.ConnectionString;

            try
            {
                sqlcon.Open();
            }
            catch (SqlException ex)
            {
                // To-do - handle sql connection errors
                Debug.WriteLine(ex.Message);
            }

            // Process items in the queue
            while (requestQueue.Count > 0 && sqlcon.State == ConnectionState.Open)
            {
                var da = new SqlDataAdapter();
                var ds = new DataSet();

                // Dequeue the SQLDataRequests and execute them on the SQL connection
                var request = (SQLDataRequest)requestQueue.Dequeue();
                request.Command.Connection = sqlcon;
                da.SelectCommand = request.Command;
                da.Fill(ds);

                // Invoke the callback for this request
                // Currently only supports single table returns
                if (null != request.NotifyRequestComplete)
                {
                    request.NotifyRequestComplete(request, ds.Tables.Count > 0 ? ds.Tables[0] : null);

                    //if (ds.Tables.Count > 0)
                    //{
                    //    request.NotifyRequestComplete(request, ds.Tables[0]);
                    //}
                    //else
                    //{
                    //    request.NotifyRequestComplete(request, null);
                    //}
                }

                request = null;
                ds = null;
                da.Dispose();
                da = null;
            }

            if (sqlcon.State != ConnectionState.Closed)
                sqlcon.Close();

            sqlcon = null;
        }

        public string GetSQLDateTime(DateTime netdatetime)
        {
            // Modifies the .net DateTime struct to the SQL expected format
            return netdatetime.ToString("yyyy-MM-dd HH:mm:ss");
        }

    }

    public delegate void RequestCallbackHandler(SQLDataRequest sender, DataTable dt);

    public delegate void RequestEnqueuHandler(SQLDataRequest sender);

    public class SQLDataRequest
    {
        public int UpdateInterval
        {
            set
            {
                if (tmrRefresh.Enabled)
                    tmrRefresh.Stop();

                tmrRefresh.Interval = value * 1000;

                tmrRefresh.Start();
            }
        }

        Timer tmrRefresh = new Timer();

        public SqlCommand Command;

        public RequestCallbackHandler NotifyRequestComplete = null;

        public RequestEnqueuHandler NotifyEnqueueRequest = null;

        public SQLDataRequest(SqlCommand cmd)
        {
            Command = cmd;
        }

        public SQLDataRequest(SqlCommand command, RequestCallbackHandler callback)
        {
            Command = command;
            NotifyRequestComplete = callback;
        }

        /// <summary>
        /// This initialize will instantiate a timer that can be used to fire update
        /// requests at a specified interval. Set 'UpdateInterval' to start timer.
        /// </summary>
        /// <param name="command">The SQLCommand to execute</param>
        /// <param name="callback">The method to be called upon completion</param>
        /// <param name="handler">Abstraction of the method used to queue the command</param>
        public SQLDataRequest(SqlCommand command, RequestCallbackHandler callback, RequestEnqueuHandler handler)
        {
            Command = command;
            NotifyRequestComplete = callback;
            NotifyEnqueueRequest = handler;

            tmrRefresh.Elapsed += SQLDataRequest_Elapsed;
        }

        void SQLDataRequest_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (null != NotifyEnqueueRequest)
                NotifyEnqueueRequest(this);
        }

    }

}
