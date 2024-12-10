using Microsoft.AspNetCore.Mvc;
using Travel_Agency___Data.ModelManagers;
using Travel_Agency___Data.Models;

namespace Travel_Agency___Web.Controllers
{
    public class PackageController : Controller
    {
        private PackageManager _packageManager;
        private TravelExpertsContext _context;

        public PackageController(TravelExpertsContext ctx)
        {
            _context = ctx;
            _packageManager = new PackageManager(_context);
        }
        public IActionResult Index()
        {
            List<Package> packages = _packageManager.GetAllPackages();
            return View(packages);
        }
    }
}
