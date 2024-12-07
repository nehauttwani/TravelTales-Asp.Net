using System.Collections.Generic;
using System.Linq;
using Travel_Agency___Data.Models;

namespace Travel_Agency___Data.ModelManagers
{
    public static class CustomerManager
    {
        public static List<Customer> GetCustomers(TravelExpertsContext db)
        {
            return db.Customers.ToList();
        }

        public static Customer GetCustomerById(TravelExpertsContext db, int customerId)
        {
            return db.Customers.FirstOrDefault(c => c.CustomerId == customerId);
        }

        public static void AddCustomer(TravelExpertsContext db, Customer customer)
        {
            db.Customers.Add(customer);
            db.SaveChanges();
        }

        public static void UpdateCustomer(TravelExpertsContext db, Customer customer)
        {
            db.Customers.Update(customer);
            db.SaveChanges();
        }

        public static void DeleteCustomer(TravelExpertsContext db, int customerId)
        {
            var customer = db.Customers.FirstOrDefault(c => c.CustomerId == customerId);
            if (customer != null)
            {
                db.Customers.Remove(customer);
                db.SaveChanges();
            }
        }
    }
}
