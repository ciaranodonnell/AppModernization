using System;
using System.Collections.Generic;
using System.Text;

namespace CDC.Loan.Events
{
    public class LoanCDCEvent
    {

        public enum CDCEventType { LoanDeleted, LoanUpsert}

        public CDCEventType EventType { get; set; }

        public LoanDeletedEvent LoanDeletedEvent { get; set; }
        public LoanUpsertEvent LoanUpsertEvent { get; set; }


    }
}
