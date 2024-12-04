using Microsoft.AspNetCore.Mvc;
using Travel_Agency___Data.ModelManagers;
using Travel_Agency___Data;
using System.Diagnostics;

namespace Travel_Agency___Web.Controllers
{
    public class ContactController : Controller
    {

        private AgentsAndAgenciesManager _agentsAndAgenciesManager;
        private TravelExpertsContext _context;

        public ContactController(TravelExpertsContext ctx)
        {
            _context = ctx;
            _agentsAndAgenciesManager = new AgentsAndAgenciesManager(_context);
        }
        public IActionResult Index()
        {
            var list = _agentsAndAgenciesManager.GetAgenciesWithAgents();
            return View(list);
        }


    }
}
