using CDC.Messaging.Core.Interfaces;
using Microsoft.Extensions.Logging;
using System;

namespace CDC.Loan
{
    public class LoanDataChangeProcessor : IDisposable
    {
        private bool disposedValue = false; // To detect redundant calls
        private readonly ILogger logger;

        private LoanDataChangeDetector loanDataChangeDetector;
        private LoanDataChangePublisher loanDataChangePublisher;

        public LoanDataChangeProcessor(ILogger logger, ISerializer serializer, string connectionString, int pollIntervalInSeconds = 10)
        {
            if (string.IsNullOrWhiteSpace(connectionString)) throw new ArgumentException("message", nameof(connectionString));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));

            loanDataChangeDetector = new LoanDataChangeDetector(logger, connectionString, pollIntervalInSeconds);
            loanDataChangeDetector.PublishLoanDeletedEvent += LoanChangeDetector_PublishLoanDeletedEvent;
            loanDataChangeDetector.PublishLoanUpsertEvent += LoanChangeDetector_PublishLoanUpsertEvent;
            loanDataChangePublisher = new LoanDataChangePublisher(logger, serializer, null, null);

        }

        private void LoanChangeDetector_PublishLoanUpsertEvent(object sender, LoanPublishEventArgs<LoanUpsertEvent> loanUpsertEvent)
        {
            throw new NotImplementedException();
        }

        private void LoanChangeDetector_PublishLoanDeletedEvent(object sender, LoanPublishEventArgs<LoanDeletedEvent> loanDeletedEvent)
        {
            throw new NotImplementedException();
        }

        public void Start()
        {
            if (loanDataChangeDetector == null) throw new InvalidOperationException($"LoanDataChangeDetector is null.");
            if (loanDataChangePublisher == null) throw new InvalidOperationException($"LoanDataChangePublisher is null.");

            loanDataChangeDetector.Start();
        }

        public void Stop()
        {
            loanDataChangeDetector.Stop();
        }

        #region IDisposable Support

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    loanDataChangeDetector.PublishLoanDeletedEvent -= LoanChangeDetector_PublishLoanDeletedEvent;
                    loanDataChangeDetector.PublishLoanUpsertEvent -= LoanChangeDetector_PublishLoanUpsertEvent;
                    loanDataChangeDetector.Stop();
                    loanDataChangeDetector = null;
                    loanDataChangePublisher = null;
                }

                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);

        }
        #endregion
    }
}
