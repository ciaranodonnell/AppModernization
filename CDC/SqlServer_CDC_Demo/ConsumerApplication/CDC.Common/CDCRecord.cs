using System;
using System.Data;

namespace CDCOutboxSender
{
    public class CDCRecord
    {
        private const string StartLSNFieldName = "__$start_lsn";
        private const string SeqValFieldName = "__$seqval";
        private const string OperationFieldName = "__$operation";
        private const string UpdateMaskFieldName = "__$update_mask";
        private const string ChangeIdFieldName = "ChangeId";


        public CDCRecord()
        {


        }



        public CDCRecord(IDataReader reader)
        {

            if (reader.IsClosed) throw new ArgumentException("You have passed a closed Data Reader", nameof(reader));


            this.StartLSN = (byte[])reader.GetValue(reader.GetOrdinal(StartLSNFieldName));
            this.LSNString = Convert.ToBase64String(this.StartLSN);
            this.Operation = (CDCOperation)reader.GetInt32(reader.GetOrdinal(OperationFieldName));
            this.UpdateMask = (byte[])reader.GetValue(reader.GetOrdinal(UpdateMaskFieldName));
            this.ChangeId = reader.GetInt64(reader.GetOrdinal(ChangeIdFieldName)); 

            //The seqval column is not there in net results but that can be OK.
            this.SeqVal = ExtractValueFromReaderIfPresent<object>(reader, SeqValFieldName);


        }

        protected T ExtractValueFromReaderIfPresent<T>(IDataReader reader, string column)
        {
            var schema = reader.GetSchemaTable();
            for (int x = 0; x < schema.Rows.Count; x++)
            {
                DataRow row = schema.Rows[x];
                if (string.CompareOrdinal(row["ColumnName"].ToString(), column) == 0)
                {
                    return (T)reader.GetValue(x);
                }
            }
            return default(T);
        }

        public string LSNString { get; internal set; }
        public byte[] StartLSN { get; private set; }
        public CDCOperation Operation { get; private set; }
        public byte[] UpdateMask { get; private set; }
        public long ChangeId { get; private set; }
        public object SeqVal { get; private set; }
    }
}
