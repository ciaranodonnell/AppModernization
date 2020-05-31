using CustomerService.Domain.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace CustomerService.Domain.Events
{
    public class CustomerUpdatedEvent : Event
    {

        public Customer UpdatedCustomer { get; set; }




    }
}
