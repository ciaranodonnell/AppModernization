using System;
using System.Collections.Generic;
using System.Text;

namespace CDC.Loan
{
    public class BaseEvent
    {

        public DateTimeOffset EventDateTime { get; set; }

        public string CorrelationId { get; set; }

        public long ChangeId { get; set; }
    }
}
