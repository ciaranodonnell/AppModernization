using CustomerService.Data;
using CustomerService.Domain.Events;
using CustomerService.Domain.Model;
using CustomerService.WebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerService.WebApp.Controllers
{
    public class CustomerController : Controller
    {
        private readonly MessageBroker _messageBroker;
        private readonly ICustomerRepository _customerRepository;

        public CustomerController(ICustomerRepository customerRepo, MessageBroker messageBroker)
        {
            _messageBroker = messageBroker;
            this._customerRepository = customerRepo;
        }

        [HttpPost]
        public ActionResult SaveCustomer(CreateCustomerViewModel customer)
        {
            try
            {
                Customer newCustomer = new Customer
                {
                    Name = customer.Name,
                    RegisteredAddress = new Address { Address1 = customer.Address1, PostalCode = customer.PostalCode },
                    Contacts = new List<Contact>() { new Contact { Name = customer.MainContact } }
                };

                _customerRepository.InsertNewCustomer(newCustomer);

                _messageBroker.SendMessage(new NewCustomerCreatedEvent
                {
                    NewCustomer = newCustomer,
                    CorrelationId = Guid.NewGuid()
                });

                return Ok(newCustomer);

            }
            catch (Exception ex)
            {
                return Redirect("Error.html");
            }
        }
    }
}
