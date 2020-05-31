using Common.Messaging.Interfaces;
using CustomerService.Data;
using CustomerService.Data.Interfaces;
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
    public class ReliableCustomerController : Controller
    {
        private readonly IMessageSender<NewCustomerCreatedEvent> messageSender;
        private readonly ICustomerRepository _customerRepository;

        public ReliableCustomerController(ICustomerRepository customerRepo,
            IMessageSender<NewCustomerCreatedEvent> messageSender)
        {
            this.messageSender = messageSender;
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

                _customerRepository.InsertNewCustomer(newCustomer, "nobody");

                messageSender.SendMessage(new NewCustomerCreatedEvent
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
