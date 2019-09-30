
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace CDC.Loan
{
    public class LoanCDCRecord : OutboxedCDCRecord
    {

        public LoanCDCRecord()
        {

        }

        public LoanCDCRecord(IDataReader reader)
            : base(reader)
        {
            LoanId = ExtractValueFromReaderIfPresent<int>(reader, "LoanId");
            PropertyId = ExtractValueFromReaderIfPresent<int>(reader, "PropertyId");
            AmountInPennies = ExtractValueFromReaderIfPresent<int>(reader, "AmountInPennies");
            RequestedCloseDate = ExtractValueFromReaderIfPresent<DateTime>(reader, "RequestedCloseDate");
        }

        public int LoanId { get; private set; }
        public int PropertyId { get; private set; }
        public int AmountInPennies { get; private set; }
        public DateTime RequestedCloseDate { get; private set; }
        
    }
}
