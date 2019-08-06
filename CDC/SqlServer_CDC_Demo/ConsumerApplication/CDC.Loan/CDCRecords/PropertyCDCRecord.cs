using CDCOutboxSender;
using System.Data;

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
            Value = ExtractValueFromReaderIfPresent<decimal>(reader, "Value");

        }


        public int PropertyId { get; private set; }
        public string Address { get; private set; }
        public decimal Value { get; private set; }

    }
}
