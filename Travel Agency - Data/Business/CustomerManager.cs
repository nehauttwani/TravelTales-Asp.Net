using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Travel_Agency___Data.Models;
using Travel_Agency___Data.ViewModels;

namespace Travel_Agency___Data.Business
{
    public static class CustomerManager
    {
        // Search customers by name 
        public static List<Customer> GetCustomersByName(TravelExpertsContext context, string name)
        {
            return context.Customers
                .Where(c => c.CustFirstName.Contains(name) || c.CustLastName.Contains(name))
                .ToList();
        }

        // Get customer details by ID
        public static Customer GetCustomerById(TravelExpertsContext context, int customerId)
        {
            return context.Customers.FirstOrDefault(c => c.CustomerId == customerId);
        }


        // Get all customers
        //public static List<Customer> GetCustomers(TravelExpertsContext context)
        //{
        //    return context.Customers.ToList();
        //}


        // Get purchases made by a specific customer
        public static List<ProductListViewModel> GetCustomerPurchases(TravelExpertsContext context, int customerId)
        {
            return context.Bookings
                .Where(b => b.CustomerId == customerId && b.Package != null)
                .Include(b => b.Package) // Ensure Packages are eagerly loaded
                 .Select(b => new ProductListViewModel
        {
                    PackageName = b.Package.PkgName,
                    PackagePrice = b.Package.PkgBasePrice,
                    AgencyCommission = b.Package.PkgAgencyCommission ?? 0
                })
                .ToList();
        }

        //public static Customer? GetCustomerById(TravelExpertsContext context, int customerId)
        //{
        //    return context.Customers
        //                  .Include(c => c.Bookings)
        //                  .ThenInclude(b => b.Package)
        //                  .FirstOrDefault(c => c.CustomerId == customerId);
        //}

        //public static List<ProductListViewModel> GetCustomerPurchases(TravelExpertsContext context, int customerId)
        //{
        //    return context.Bookings
                           
        //                  .Where(b => b.CustomerId == customerId)
        //                  .Select(b => new ProductListViewModel
        //                  {
        //                      PackageName = b.Package.PkgName,
        //                      PackagePrice = b.Package.PkgBasePrice,
        //                      AgencyCommission = b.Package.PkgAgencyCommission ?? 0m,
        //                      // TotalCost is calculated automatically  in the ProductListViewModel
        //                  })
        //                  .ToList();
        //}
    }
}
