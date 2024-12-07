using Microsoft.AspNetCore.Mvc;
using Travel_Agency___Data;

namespace Travel_Agency___Web.Controllers
{
    public class CreditCardController : Controller
    {
        private readonly TravelExpertsContext _context;

        public CreditCardController(TravelExpertsContext context)
        {
            _context = context;
        }

        // View credit cards
        public IActionResult Index(int customerId)
        {
            var creditCards = _context.CreditCards
                                      .Where(cc => cc.CustomerId == customerId)
                                      .ToList();
            return View(creditCards);
        }
    }
}
