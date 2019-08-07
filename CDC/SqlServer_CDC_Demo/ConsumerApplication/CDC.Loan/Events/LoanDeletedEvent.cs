namespace CDC.Loan
{
    public class LoanDeletedEvent : BaseEvent
    {
        public int LoanId { get; set; }
        
    }
}