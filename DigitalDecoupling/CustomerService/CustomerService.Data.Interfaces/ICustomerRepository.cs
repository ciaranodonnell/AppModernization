using CustomerService.Domain.Model;
using System;

namespace CustomerService.Data.Interfaces
{
    public interface ICustomerRepository 
    {

        Customer InsertNewCustomer(Customer customer, string UserId);
        Customer InsertNewCustomerAndEvent(Customer customer, string UserId);

    }
}
