using CDC.Common;
using CDCOutboxSender;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;

namespace CDC.Loan
{
    public class LoanDataChangeDetector : IDisposable
    {
        private readonly AutoResetEvent changeDetectionProcessCompletion = new AutoResetEvent(false);
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ILogger logger;

        public event EventHandler<LoanPublishEventArgs<LoanDeletedEvent>> PublishLoanDeletedEvent;
        public event EventHandler<LoanPublishEventArgs<LoanUpsertEvent>> PublishLoanUpsertEvent;

        public LoanDataChangeDetector(ILogger logger, string connectionString, int pollIntervalInSeconds = 10)
        {
            if (string.IsNullOrWhiteSpace(connectionString)) throw new ArgumentException("message", nameof(connectionString));

            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.ConnectionString = connectionString;
            this.PollIntervalInSeconds = pollIntervalInSeconds;

        }

        public int PollIntervalInSeconds { get; }

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

            foreach (var lsn in orderedChanges)
            {
                CDCRecord record = allChangeRecords[lsn];

                if (record is LoanCDCRecord loancdc)
                {
                    SendLoanEvent(loancdc);
                }
                else if (record is LoanApplicantCDCRecord loanapplicantcdc)
                {
                    SendLoanApplicantEvent(loanapplicantcdc);
                }
                else if (record is PropertyCDCRecord propertyCdc)
                {
                    SendPropertyEvent(propertyCdc);
                }
                else if (record is ApplicantCDCRecord applicantCdc)
                {
                    SendApplicantEvent(applicantCdc);
                }
            }

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
                    var loanDeletedEvent = new LoanDeletedEvent { CorrelationId = Guid.NewGuid().ToString(), LoanId = cdc.LoanId };

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
                        EventDateTime = DateTime.UtcNow,
                        PropertyId = cdc.PropertyId,
                        RequestedCloseDate = cdc.RequestedCloseDate
                    };

                    // publish Event
                    PublishLoanUpsertEvent?.Invoke(this, new LoanPublishEventArgs<LoanUpsertEvent>(loanUpsertEvent));

                    break;


            }
        }

        /// <summary>
        /// Function to hide the details about running an SP
        /// </summary>
        /// <param name="procedureName">the name of the SP to run</param>
        /// <param name="connectionString">a connection string to the SQL Server database</param>
        /// <param name="loadDataAction">a function to call that will extract the data from the reader. The data items will likely be in a closure for this to work</param>
        private void RunStoredProcedure(string procedureName, string connectionString, Action<IDataReader> loadDataAction)
        {
            try
            {
                using (var connection = CDCHelper.GetConnection(ConnectionString))
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = procedureName;
                    command.CommandType = CommandType.StoredProcedure;
                    connection.Open();
                    var reader = command.ExecuteReader();

                    loadDataAction(reader);

                }
            }
            catch (Exception ex)
            {
                //TODO:handle errors here and decide what to do
                throw;
            }
        }


        /// <summary>
        /// Start the process checking for changes periodically
        /// </summary>
        public void Start()
        {
            //TODO: Revisit to make sure only one instance runs...may be make the hosting thread static instance
            // if (isRunning) throw new InvalidOperationException("You are starting a ChangeDetector that is already running");

            new Thread(new ThreadStart(() =>
            {
                while (!cancellationTokenSource.IsCancellationRequested)
                {
                    var changeDetectorProcessThread = new Thread(new ThreadStart(StartChangeDetection));
                    changeDetectorProcessThread.Start();

                    this.logger.LogInformation($"Now Waiting for Change Detection thread:  {changeDetectorProcessThread.ManagedThreadId}");
                    changeDetectionProcessCompletion.WaitOne();
                }
            })).Start();

        }

        private void StartChangeDetection()
        {
            try
            {
                CheckForAndPublishChanges();
            }
            catch (Exception ex)
            {
                this.logger.LogError($"{ex.Message}");
                throw;
            }
            finally
            {
                changeDetectionProcessCompletion.Set();
                this.logger.LogInformation($"Releasing Change Detection Thread: {Thread.CurrentThread.ManagedThreadId}");
            }
        }


        /// <summary>
        /// Stop the process from running periodically
        /// </summary>
        public void Stop()
        {
            if (!cancellationTokenSource.IsCancellationRequested)
                cancellationTokenSource.Cancel();
        }

        public void Dispose()
        {
            Stop();

            changeDetectionProcessCompletion?.Dispose();
            cancellationTokenSource?.Dispose();
        }
    }

    public class LoanPublishEventArgs<TEventType> : EventArgs
    {
        public LoanPublishEventArgs(TEventType eventType)
        {
            EventType = eventType;
        }
        public TEventType EventType { get; set; }
    }
}
