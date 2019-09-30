
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace CDC.Loan
{
    public class LoanApplicantCDCRecord : OutboxedCDCRecord
    {

        public LoanApplicantCDCRecord()
        {

        }
        
        public LoanApplicantCDCRecord(IDataReader reader)
            : base(reader)
        {
            LoanId = ExtractValueFromReaderIfPresent<int>(reader, "LoanId");
            ApplicantId = ExtractValueFromReaderIfPresent<int>(reader, "ApplicantId");
        }

        public int LoanId { get; private set; }
        public int ApplicantId { get; private set; }
    }
}
