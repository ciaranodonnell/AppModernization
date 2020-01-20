using System;
using System.Collections.Generic;
using System.Text;

namespace CustomerService.Domain.Model
{
    public class Entity
    {
        public int Id { get; set; }

        public int Version { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset LastUpdatedDate { get; set; }
        public string LastUpdatedByUser { get; set; }
    }
}
