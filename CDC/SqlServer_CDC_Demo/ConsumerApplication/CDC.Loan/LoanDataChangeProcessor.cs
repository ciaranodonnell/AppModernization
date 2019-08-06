using System;

namespace CDC.Loan
{
    public class LoanDataChangeProcessor : IDisposable
    {
        private LoanDataChangeDetector loanDataChangeDetector;
        private LoanDataChangePublisher loanDataChangePublisher;

        public LoanDataChangeProcessor(string connectionString, int pollIntervalInSeconds = 10)
        {
            loanDataChangeDetector = new LoanDataChangeDetector(connectionString, pollIntervalInSeconds);
            loanDataChangeDetector.PublishLoanDeletedEvent += LoanChangeDetector_PublishLoanDeletedEvent;
            loanDataChangeDetector.PublishLoanUpsertEvent += LoanChangeDetector_PublishLoanUpsertEvent;
            loanDataChangePublisher = new LoanDataChangePublisher();
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


        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls


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
