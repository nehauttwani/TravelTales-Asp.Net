using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Travel_Agency___Data.Models;
using Travel_Agency___Data.ViewModels;

namespace Travel_Agency___Data.Services
{
    // Travel_Agency___Data/Services/TravelPreferenceService.cs
    public class TravelPreferenceService
    {
        private readonly TravelExpertsContext _context;
        private readonly ILogger<TravelPreferenceService> _logger;

        public TravelPreferenceService(TravelExpertsContext context, ILogger<TravelPreferenceService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<TravelPreference?> GetPreferencesAsync(int customerId)
        {
            return await _context.TravelPreferences
                .FirstOrDefaultAsync(p => p.CustomerId == customerId);
        }

        public async Task UpdatePreferencesAsync(TravelPreference preference)
        {
            var existing = await _context.TravelPreferences
                .FirstOrDefaultAsync(p => p.CustomerId == preference.CustomerId);

            if (existing == null)
            {
                preference.CreatedDate = DateTime.Now;
                _context.TravelPreferences.Add(preference);
            }
            else
            {
                existing.PreferredClimate = preference.PreferredClimate;
                existing.Activities = preference.Activities;
                existing.TravelCompanion = preference.TravelCompanion;
                existing.PreferredLocation = preference.PreferredLocation;
                existing.UpdatedDate = DateTime.Now;
            }

            await _context.SaveChangesAsync();
        }
    }
}
