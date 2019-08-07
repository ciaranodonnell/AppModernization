using CDC.Messaging.Core.Interfaces;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;

namespace CDC.Loan
{
    public class LoanDataChangeProcessor : IDisposable
    {
        private bool disposedValue = false; // To detect redundant calls
        private readonly ILogger logger;

        private readonly AutoResetEvent changeDetectionProcessCompletion = new AutoResetEvent(false);
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();


        private LoanDataChangeDetector loanDataChangeDetector;
        private LoanDataChangePublisher loanDataChangePublisher;
        private Thread processingThread;

        public int PollIntervalInSeconds { get; private set; }

        public LoanDataChangeProcessor(ILogger logger, ISerializer serializer, ProducerConfig producerConfig, ConsumerConfig consumerConfig, string connectionString, int pollIntervalInSeconds = 10)
        {
            if (string.IsNullOrWhiteSpace(connectionString)) throw new ArgumentException("message", nameof(connectionString));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));

            loanDataChangeDetector = new LoanDataChangeDetector(logger, connectionString);
            loanDataChangeDetector.PublishLoanDeletedEvent += LoanChangeDetector_PublishLoanDeletedEvent;
            loanDataChangeDetector.PublishLoanUpsertEvent += LoanChangeDetector_PublishLoanUpsertEvent;
            loanDataChangePublisher = new LoanDataChangePublisher(logger, serializer, producerConfig, consumerConfig);


            this.PollIntervalInSeconds = pollIntervalInSeconds;
        }

        private void LoanChangeDetector_PublishLoanUpsertEvent(object sender, LoanPublishEventArgs<LoanUpsertEvent> loanUpsertEvent)
        {
            // throw new NotImplementedException();
        }

        private void LoanChangeDetector_PublishLoanDeletedEvent(object sender, LoanPublishEventArgs<LoanDeletedEvent> loanDeletedEvent)
        {
            // throw new NotImplementedException();
        }

        public void Start()
        {
            if (loanDataChangeDetector == null) throw new InvalidOperationException($"LoanDataChangeDetector is null.");
            if (loanDataChangePublisher == null) throw new InvalidOperationException($"LoanDataChangePublisher is null.");

            //TODO: Revisit to make sure only one instance runs...may be make the hosting thread static instance
            // if (isRunning) throw new InvalidOperationException("You are starting a ChangeDetector that is already running");

            this.processingThread = new Thread(new ThreadStart(() =>
            {
                while (!cancellationTokenSource.IsCancellationRequested)
                {
                    var changeDetectorProcessThread = new Thread(new ThreadStart(StartChangeDetection));
                    changeDetectorProcessThread.Start();

                    this.logger.LogInformation($"Now Waiting for Change Detection thread:  {changeDetectorProcessThread.ManagedThreadId}");
                    changeDetectionProcessCompletion.WaitOne();
                }
            }));
            processingThread.Start();

        }


        private void StartChangeDetection()
        {
            try
            {
                loanDataChangeDetector.CheckForAndPublishChanges();
            }
            catch (Exception ex)
            {
                this.logger.LogError($"{ex.Message}");
                throw;
            }
            finally
            {
                //TODO: Move this to try body and decide what to do with the process when there is an unhandled exception
                changeDetectionProcessCompletion.Set();
                this.logger.LogInformation($"Releasing Change Detection Thread: {Thread.CurrentThread.ManagedThreadId}");
            }
        }

        /// Stop the process from running periodically
        /// </summary>
        public void Stop()
        {
            if (!cancellationTokenSource.IsCancellationRequested)
                cancellationTokenSource.Cancel();
        }

        #region IDisposable Support

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Stop();

                    loanDataChangeDetector.PublishLoanDeletedEvent -= LoanChangeDetector_PublishLoanDeletedEvent;
                    loanDataChangeDetector.PublishLoanUpsertEvent -= LoanChangeDetector_PublishLoanUpsertEvent;

                    loanDataChangeDetector = null;
                    loanDataChangePublisher.Dispose();
                    changeDetectionProcessCompletion.Dispose();
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
