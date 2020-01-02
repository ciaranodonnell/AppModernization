using System;
using System.Collections.Generic;
using System.Text;

namespace CustomerService.Domain.Events
{
    public class Event
    {

        public Guid CorrelationId { get; set; }

        public DateTimeOffset EventRaised { get; set; }

        public string EventId { get; set; }

    }
}
