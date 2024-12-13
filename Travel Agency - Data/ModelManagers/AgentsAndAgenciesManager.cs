using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Travel_Agency___Data.Models;

namespace Travel_Agency___Data.ModelManagers
{
    public class AgentsAndAgenciesManager
    {
        private readonly TravelExpertsContext _context;

        public AgentsAndAgenciesManager(TravelExpertsContext ctx)
        {
            _context = ctx ?? throw new ArgumentNullException(nameof(ctx), "Database context cannot be null");
        }

        public IEnumerable<Agency> GetAgenciesWithAgents()
        {
            try
            {
                return _context.Agencies
                    .Include(a => a.Agents)
                    .OrderBy(a => a.AgencyId)
                    .ThenBy(a => a.AgncyCity)
                    .ToList() ?? new List<Agency>();
            }
            catch (Exception ex)
            {
                // Log the error if you have logging configured
                // logger.LogError(ex, "Error retrieving agencies with agents");
                return new List<Agency>();
            }
        }

        public List<Agent> GetAgents()
        {
            try
            {
                return _context.Agents
                    .OrderBy(a => a.AgtFirstName)
                    .ThenBy(a => a.AgtLastName)
                    .Select(a => new Agent
                    {
                        AgentId = a.AgentId,
                        AgtFirstName = a.AgtFirstName,
                        AgtLastName = a.AgtLastName,
                        AgtMiddleInitial = a.AgtMiddleInitial,
                        AgtBusPhone = a.AgtBusPhone,
                        AgtEmail = a.AgtEmail,
                        AgtPosition = a.AgtPosition,
                        AgencyId = a.AgencyId,
                        // Include other properties you need
                    })
                    .ToList() ?? new List<Agent>();
            }
            catch (Exception ex)
            {
                // Log the error if you have logging configured
                // logger.LogError(ex, "Error retrieving agents");
                return new List<Agent>();
            }
        }

        // Optional: Add a method to get agent details with agency information
        public Agent GetAgentWithAgency(int agentId)
        {
            try
            {
                return _context.Agents
                    .Include(a => a.Agency)
                    .FirstOrDefault(a => a.AgentId == agentId) ?? new Agent();
            }
            catch (Exception ex)
            {
                // Log the error if you have logging configured
                // logger.LogError(ex, $"Error retrieving agent with ID {agentId}");
                return new Agent();
            }
        }

        // Optional: Add a method to get agents by agency
        public List<Agent> GetAgentsByAgency(int agencyId)
        {
            try
            {
                return _context.Agents
                    .Where(a => a.AgencyId == agencyId)
                    .OrderBy(a => a.AgtFirstName)
                    .ThenBy(a => a.AgtLastName)
                    .ToList() ?? new List<Agent>();
            }
            catch (Exception ex)
            {
                // Log the error if you have logging configured
                // logger.LogError(ex, $"Error retrieving agents for agency {agencyId}");
                return new List<Agent>();
            }
        }
    }
}