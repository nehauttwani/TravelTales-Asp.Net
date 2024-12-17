using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Travel_Agency___Data.Models;

namespace Travel_Agency___Data.ModelManagers
{
    public class PackageManager
    {
        private TravelExpertsContext _context { get; set; }

        public PackageManager(TravelExpertsContext ctx)
        {
            _context = ctx;
        }

        public List<Package> GetAllPackages()
        {
            return _context.Packages
                          .Where(p => !p.IsDeleted)
                          .ToList();
        }

        public Package GetPackage(int packageID) =>
            _context.Packages
                   .Where(p => !p.IsDeleted && p.PackageId == packageID)
                   .FirstOrDefault();
    }
}