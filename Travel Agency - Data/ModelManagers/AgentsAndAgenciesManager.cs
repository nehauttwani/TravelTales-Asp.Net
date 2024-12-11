using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Travel_Agency___Data.Models;

namespace Travel_Agency___Data.ModelManagers
{
    public class AgentsAndAgenciesManager
    {
        private TravelExpertsContext _context;

        // Constructor
        public AgentsAndAgenciesManager(TravelExpertsContext ctx)
        {
            _context = ctx ?? throw new ArgumentNullException(nameof(ctx), "Database context cannot be null");
        }

        // Method to get agencies along with agents
        public IEnumerable<dynamic> GetAgenciesWithAgents()
        {
            var agencies = _context.Agencies
                .Include(a => a.Agents)
                .OrderBy(a => a.AgencyId)
                .ThenBy(a => a.AgncyCity)
                .ToList();

            return agencies;
        }

        // Method to get all agents, ensuring it doesn't return null
        public List<Agent> GetAgents()
        {
            var agents = _context.Agents.OrderBy(a => a.AgtFirstName).ToList();
            return agents ?? new List<Agent>(); // Ensuring it returns an empty list if null
        }
    }
}
