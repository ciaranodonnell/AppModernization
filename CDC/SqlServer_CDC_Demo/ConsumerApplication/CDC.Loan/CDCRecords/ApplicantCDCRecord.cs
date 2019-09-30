
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace CDC.Loan
{
    public class ApplicantCDCRecord : OutboxedCDCRecord
    {

        public ApplicantCDCRecord()
        {

        }

        public ApplicantCDCRecord(IDataReader reader)
            : base(reader)
        {
            ApplicantId = ExtractValueFromReaderIfPresent<int>(reader, "ApplicantId");
            DateOfBirth = ExtractValueFromReaderIfPresent<DateTime>(reader, "DateOfBirth");
            CreditScore = ExtractValueFromReaderIfPresent<int>(reader, "CreditScore");
            Name = ExtractValueFromReaderIfPresent<string>(reader, "Name");
        }

        public int ApplicantId { get; private set; }
        public DateTime DateOfBirth { get; private set; }
        public int CreditScore { get; private set; }
        public string Name { get; private set; }
    }
}
