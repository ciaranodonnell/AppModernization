using CDC.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace CDC.Loan
{
   public class OutboxedCDCRecord : CDCRecord
    {

        private const string ChangeIdFieldName = "ChangeId";

        public OutboxedCDCRecord()
        {

        }

        public OutboxedCDCRecord(IDataReader reader)
            : base(reader)
        {
            this.ChangeId = reader.GetInt64(reader.GetOrdinal(ChangeIdFieldName));
        }


        public long ChangeId { get; private set; }
    }
}
