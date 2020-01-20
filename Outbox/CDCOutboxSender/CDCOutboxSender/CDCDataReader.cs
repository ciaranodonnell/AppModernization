using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace CDCOutboxSender
{
    public class CDCDataReader
    {
        private Timer timer;
        private int batchSize;
        private long? lastSuccessfulSend;

        internal void Start()
        {
            throw new NotImplementedException();
        }

        private int timeBetweenBatchesInSeconds;
        private string connectionString;
        private string auditTableName;
        private string auditSchemaName;
        private SqlConnection connection;
        private Func<IEnumerable<CDCRecord>, long?> handler;
        private Action<Exception> errorHandler;

        public bool Started { get; private set; }

        public CDCDataReader(string tableName, string schemaName, string dbConnectionString, long? lastSuccessfulSend, int timeBetweenBatchesInSeconds = 5, int maxBatchSize = 100)
        {
            this.timer = new System.Threading.Timer(DoCheck, null, System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
            this.batchSize = maxBatchSize;
            this.lastSuccessfulSend = lastSuccessfulSend;
            this.timeBetweenBatchesInSeconds = timeBetweenBatchesInSeconds;
            this.connectionString = dbConnectionString;
            this.auditTableName = tableName;
            this.auditSchemaName = schemaName;
            this.connection = new SqlConnection(connectionString);
        }

        private void DoCheck(object state)
        {
            try
            {
                connection.Open();

                SqlCommand cmd = new SqlCommand("GetAuditOutboxData", connection);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add("@AuditTableName", System.Data.SqlDbType.VarChar, 200).Value = auditTableName;
                cmd.Parameters.Add("@AuditSchema", System.Data.SqlDbType.VarChar, 200).Value = auditSchemaName;
                cmd.Parameters.Add("@LastProcessedRecord", System.Data.SqlDbType.BigInt).Value = lastSuccessfulSend;
                cmd.Parameters.Add("@MaxBatchSize", System.Data.SqlDbType.Int, 200).Value = batchSize;

                var reader = cmd.ExecuteReader();
                List<CDCRecord> records = new List<CDCRecord>();
                if (reader.Read())
                {
                    var schema = reader.GetSchemaTable();
                    do
                    {
                        Dictionary<string, object> fields = new Dictionary<string, object>();
                        long updateId = 0;
                        DateTimeOffset updated = DateTimeOffset.UtcNow;
                        string updatedBy = "";
                        UpdateType updateType = UpdateType.Insert;
                        int x = 0;
                        foreach (DataRow field in schema.Rows)
                        {

                            switch (field[0].ToString())
                            {
                                case "Audit_UpdateNumber":
                                    updateId = reader.GetInt64(x);
                                    break;
                                case "Audit_UpdatedBy":
                                    updatedBy = reader.GetString(x);
                                    break;
                                case "Audit_Updated":
                                    updated = reader.GetDateTimeOffset(x);
                                    break;
                                case "Audit_UpdateType":
                                    var c = reader.GetString(x)[0];
                                    switch (c)
                                    {
                                        case 'U': updateType = UpdateType.Update; break;
                                        case 'I': updateType = UpdateType.Insert; break;
                                        case 'D': updateType = UpdateType.Delete; break;
                                    }
                                    break;
                                default:
                                    fields.Add(field[0].ToString(), reader.GetValue(x));
                                    break;

                            }

                            x++;


                            records.Add(new CDCRecord(fields, updateId, updateType, updatedBy, updated));

                        }
                    } while (reader.Read());

                    connection.Close();

                    this.lastSuccessfulSend = handler(records);

                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception Occured checking for Change Data: " + ex.ToString(), "ERROR");
                errorHandler?.Invoke(ex);
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="handler"></param>
        public void Start(Func<IEnumerable<CDCRecord>, long?> handler, Action<Exception> errorHandler)
        {

            if (this.Started) throw new InvalidOperationException("This CDCDataReader is already running");

            if (handler == null) throw new ArgumentNullException(nameof(handler));
            this.handler = handler;
            this.errorHandler = errorHandler;

            var period = TimeSpan.FromSeconds(timeBetweenBatchesInSeconds);
            this.timer.Change(period, period);
            Started = true;

        }


        public void Stop()
        {
            this.timer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
            Started = false;
        }


    }

}