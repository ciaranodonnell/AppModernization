using System;

namespace CDC.Loan
{
    public class LoanUpsertEvent : BaseEvent
    {

        public int LoanId { get; set; }
        public int PropertyId { get; set; }

        public int AmountInPennies { get; set; }

        public DateTime? RequestedCloseDate { get; set; }
        
    }
}
