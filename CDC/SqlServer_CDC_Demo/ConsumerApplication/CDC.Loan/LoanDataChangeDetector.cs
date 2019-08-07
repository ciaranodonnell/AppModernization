using CDC.Common;
using CDCOutboxSender;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;

namespace CDC.Loan
{
    public class LoanDataChangeDetector
    {
        private readonly ILogger logger;

        public event EventHandler<LoanPublishEventArgs<LoanDeletedEvent>> PublishLoanDeletedEvent;
        public event EventHandler<LoanPublishEventArgs<LoanUpsertEvent>> PublishLoanUpsertEvent;

        public LoanDataChangeDetector(ILogger logger, string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString)) throw new ArgumentException("message", nameof(connectionString));

            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.ConnectionString = connectionString;

        }

        public string ConnectionString { get; }

        /// <summary>
        /// This is the method that runs the actual process
        /// </summary>
        public void CheckForAndPublishChanges()
        {
            this.logger.LogInformation($"Running Change Detection on thread: {Thread.CurrentThread.ManagedThreadId}");

            Dictionary<string, CDCRecord> allChangeRecords = new Dictionary<string, CDCRecord>();
            List<string> orderedChanges = new List<string>();

            RunStoredProcedure("GetUpdatedLoanEntities", ConnectionString, (reader) =>
            {
                while (reader.Read())
                {
                    var record = new LoanCDCRecord(reader);
                    allChangeRecords.Add(record.LSNString, record);
                }
                reader.NextResult();
                while (reader.Read())
                {
                    var record = new ApplicantCDCRecord(reader);
                    allChangeRecords.Add(record.LSNString, record);
                }
                reader.NextResult();
                while (reader.Read())
                {
                    var record = new LoanApplicantCDCRecord(reader);
                    allChangeRecords.Add(record.LSNString, record);
                }
                reader.NextResult();
                while (reader.Read())
                {
                    var record = new PropertyCDCRecord(reader);
                    allChangeRecords.Add(record.LSNString, record);
                }


                reader.NextResult();

                while (reader.Read())
                {
                    orderedChanges.Add(Convert.ToBase64String((byte[])reader.GetValue(0)));
                }


            });

            DataTable sentMessages = new DataTable()
            {
                Columns = {
                    new DataColumn("ChangeId", typeof(Int64)),
                    new DataColumn("EventSentUTC", typeof(DateTimeOffset))
                }
            };

            foreach (var lsn in orderedChanges)
            {
                CDCRecord record = allChangeRecords[lsn];

                if (record is LoanCDCRecord loancdc)
                {
                    SendLoanEvent(loancdc);
                    sentMessages.Rows.Add(record.ChangeId, DateTimeOffset.UtcNow);
                }
                else if (record is LoanApplicantCDCRecord loanapplicantcdc)
                {
                    SendLoanApplicantEvent(loanapplicantcdc);
                    sentMessages.Rows.Add(record.ChangeId, DateTimeOffset.UtcNow);
                }
                else if (record is PropertyCDCRecord propertyCdc)
                {
                    SendPropertyEvent(propertyCdc);
                    sentMessages.Rows.Add(record.ChangeId, DateTimeOffset.UtcNow);
                }
                else if (record is ApplicantCDCRecord applicantCdc)
                {
                    SendApplicantEvent(applicantCdc);
                    sentMessages.Rows.Add(record.ChangeId, DateTimeOffset.UtcNow);
                }
            }

            //Store the record in outbox postmarks
            if (sentMessages.Rows.Count > 0)
                RunStoredProcedure("cdc.StoreOutboxPostmark", ConnectionString, null, new Dictionary<string, object> { { "@postmarks", sentMessages } });


        }

        private void SendPropertyEvent(PropertyCDCRecord propertyCdc)
        {

        }

        private void SendApplicantEvent(ApplicantCDCRecord applicantCdc)
        {

        }

        private void SendLoanApplicantEvent(LoanApplicantCDCRecord loanapplicantcdc)
        {

        }

        private void SendLoanEvent(LoanCDCRecord cdc)
        {

            switch (cdc.Operation)
            {
                case CDCOperation.Delete:
                    var loanDeletedEvent = new LoanDeletedEvent { CorrelationId = Guid.NewGuid().ToString(), LoanId = cdc.LoanId, ChangeId = cdc.ChangeId };

                    // publish Event
                    PublishLoanDeletedEvent?.Invoke(this, new LoanPublishEventArgs<LoanDeletedEvent>(loanDeletedEvent));

                    break;
                case CDCOperation.Insert:
                case CDCOperation.Upsert:
                case CDCOperation.UpdateAfterChange:
                case CDCOperation.UpdateBeforeChange:
                    var loanUpsertEvent = new LoanUpsertEvent
                    {
                        CorrelationId = Guid.NewGuid().ToString(),
                        LoanId = cdc.LoanId,
                        AmountInPennies = cdc.AmountInPennies,
                        //getting UtcNow repeatedly means the time will be going up. we may want to get it from the EventBatchDate in the DB
                        EventDateTime = DateTime.UtcNow,
                        PropertyId = cdc.PropertyId,
                        RequestedCloseDate = cdc.RequestedCloseDate,
                        ChangeId = cdc.ChangeId
                    };

                    // publish Event
                    var args = new LoanPublishEventArgs<LoanUpsertEvent>(loanUpsertEvent);
                    PublishLoanUpsertEvent?.Invoke(this, args);

                    break;


            }
        }


        /// <summary>
        /// Function to hide the details about running an SP
        /// </summary>
        /// <param name="procedureName">the name of the SP to run</param>
        /// <param name="connectionString">a connection string to the SQL Server database</param>
        /// <param name="loadDataAction">a function to call that will extract the data from the reader. The data items will likely be in a closure for this to work</param>
        private void RunStoredProcedure(string procedureName, string connectionString, Action<IDataReader> loadDataAction, Dictionary<string, object> parameters = null)
        {
            try
            {
                using (var connection = CDCHelper.GetConnection(ConnectionString))
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = procedureName;
                    command.CommandType = CommandType.StoredProcedure;

                    if (parameters != null)
                    {
                        foreach (var p in parameters)
                        {
                            var param = command.Parameters.AddWithValue(p.Key, p.Value);
                            if (p.Value is DataTable) param.SqlDbType = SqlDbType.Structured;
                        }
                    }


                    connection.Open();

                    if (loadDataAction != null)
                    {
                        var reader = command.ExecuteReader();

                        loadDataAction(reader);
                    }
                    else
                    {
                        command.ExecuteNonQuery();
                    }

                }
            }
            catch (Exception ex)
            {
                //TODO:handle errors here and decide what to do
                throw;
            }
        }



    }

    public class LoanPublishEventArgs<TEventType> : EventArgs
    {
        public LoanPublishEventArgs(TEventType eventType)
        {
            EventType = eventType;
        }
        public TEventType EventType { get; set; }

        public bool EventPublishedSuccessfully { get; set; }
    }
}
