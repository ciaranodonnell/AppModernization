namespace CustomerService.WebApp.Models
{
    public class CreateCustomerRequest
    {

        public string Name { get; set; }

        public string Address1 { get; set; }

        public string Address2 { get; set; }


        public string Address3 { get; set; }
        public string TownCity { get; set; }
        public string County { get; set; }
        public string CountryCode { get; set; }

        public string Country { get; set; }

        public string PostalCode { get; set; }


        public string MainContact { get; set; }

        public string IndustryChosen { get; set; }

    }
}