using System;
using System.Collections.Generic;
using System.Text;

namespace CDCOutboxSender
{
    public class CDCRecord
    {
        public CDCRecord(Dictionary<string, object> data, long updateId, UpdateType updateType, string updatedBy, DateTimeOffset updatedDateUTC)
        {
            Data = data;
            UpdateId = updateId;
            UpdateType = updateType;
            UpdatedBy = updatedBy;
            UpdatedDateUTC = updatedDateUTC;
        }

        public Dictionary<string, object> Data { get; private set; }

        public long UpdateId { get; private set; }
        public UpdateType UpdateType{ get; private set; }
        
        public string UpdatedBy { get; set; }
        public DateTimeOffset UpdatedDateUTC { get; set; }

    }
}
