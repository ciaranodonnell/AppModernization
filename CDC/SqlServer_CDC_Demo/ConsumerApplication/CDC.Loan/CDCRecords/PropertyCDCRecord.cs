using CDCOutboxSender;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace CDC.Loan
{
    public class PropertyCDCRecord : CDCRecord
    {
        public PropertyCDCRecord()
        {

        }

        public PropertyCDCRecord(IDataReader reader)
            : base(reader)
        {
            PropertyId = ExtractValueFromReaderIfPresent<int>(reader, "PropertyId");
            Address = ExtractValueFromReaderIfPresent<string>(reader, "Address");
            Value = ExtractValueFromReaderIfPresent<int>(reader, "Value");

        }


        public int PropertyId { get; private set; }
        public string Address { get; private set; }
        public int Value { get; private set; }

    }
}
