using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Travel_Agency___Data.ModelManagers
{
    public class AgentsAndAgenciesManager
    {
        private TravelExpertsContext _context { get; set; }

        public AgentsAndAgenciesManager(TravelExpertsContext ctx)
        {
            _context = ctx;
        }

        public IEnumerable<dynamic> GetAgenciesWithAgents()
        {
            var agencies =  _context.Agencies
             .Include(a => a.Agents)
             .OrderBy(a => a.AgencyId)
             .ThenBy(a => a.AgncyCity)
             .ToList();

            return agencies.ToList();
        }
    }
}
