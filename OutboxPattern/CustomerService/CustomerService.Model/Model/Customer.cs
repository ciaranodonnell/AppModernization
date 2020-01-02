using System;
using System.Collections.Generic;
using System.Text;

namespace CustomerService.Domain.Model
{
    public class Customer: Entity
    {
        
        public string Name { get; set; }

        public Address RegisteredAddress { get; set; }

        public Industry PrimaryIndustry { get; set; }

        public IEnumerable<Contact> Contacts { get; set; }

    }
}
