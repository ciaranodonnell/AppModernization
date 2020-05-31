using CustomerService.Domain.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace CustomerService.Domain.Events
{
    public class NewCustomerCreatedEvent : Event
    {

        public Customer NewCustomer { get; set; }

    }
}
