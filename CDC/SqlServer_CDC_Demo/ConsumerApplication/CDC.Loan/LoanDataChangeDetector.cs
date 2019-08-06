using CDC.Common;
using CDCOutboxSender;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading;

namespace CDC.Loan
{
    public class LoanDataChangeDetector : IDisposable
    {
        private bool isRunning;
        private Timer timer;
        private object processingLockObject = new Object();

        public LoanDataChangeDetector(string connectionString, int pollIntervalInSeconds = 10)
        {
            this.PollIntervalInSeconds = pollIntervalInSeconds;
            this.ConnectionString = connectionString;
        }

        public int PollIntervalInSeconds { get; }
        public string ConnectionString { get; }



        /// <summary>
        /// This is the method that runs the actual process
        /// </summary>
        public void CheckForAndPublishChanges()
        {

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

                    

                    break;


            }
        }

        private void RunStoredProcedure(string procedureName, string connectionString, Action<IDataReader> loadDataAction)
        {
            try
            {
                using (var connection = CDCHelper.GetConnection(ConnectionString))
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = procedureName;
                    command.CommandType = CommandType.StoredProcedure;
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
            if (isRunning) throw new InvalidOperationException("You are starting a ChangeDetector that is already running");

            isRunning = true;

            this.timer = new Timer(Timer_Tick, null, 0, PollIntervalInSeconds * 1000);

        }

        void Timer_Tick(object state)
        {
            lock (processingLockObject)
            {
                if (isRunning)
                {
                    CheckForAndPublishChanges();
                }
            }
        }


        /// <summary>
        /// Stop the process from running periodically
        /// </summary>
        public void Stop()
        {
            if (isRunning)
            {
                this.timer.Change(Timeout.Infinite, Timeout.Infinite);
                this.timer.Dispose();

                isRunning = false;

            }
        }

        public void Dispose()
        {
            if (isRunning)
                Stop();
        }
    }
}
