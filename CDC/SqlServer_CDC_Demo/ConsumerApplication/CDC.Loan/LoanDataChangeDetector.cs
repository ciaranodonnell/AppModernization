using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace CDC.Loan
{
    public class LoanDataChangeDetector : IDisposable
    {
        private bool isRunning;
        private Timer timer;

        public LoanDataChangeDetector(string connectionString, int pollIntervalInSeconds = 10)
        {
            this.PollIntervalInSeconds = pollIntervalInSeconds;
            this.ConnectionString = connectionString;
        }

        public int PollIntervalInSeconds { get; }
        public string ConnectionString { get; }


        public void Start()
        {
            if (isRunning) throw new InvalidOperationException("You are starting a ChangeDetector that is already running");

            isRunning = true;

            this.timer = new Timer(Timer_Tick, null, 0, PollIntervalInSeconds * 1000);

        }

        void Timer_Tick(object state)
        {




        }


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
