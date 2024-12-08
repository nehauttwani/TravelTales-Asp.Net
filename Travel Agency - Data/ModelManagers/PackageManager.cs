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
            return _context.Packages.ToList();
        }

        public Package GetPackage(int packageID) => _context.Packages.Find(packageID);
    }
}
