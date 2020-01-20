using CustomerService.Domain.Model;
using System;

namespace CustomerService.Data
{
    public interface ICustomerRepository 
    {

        Customer InsertNewCustomer(Customer customer);

    }
}
