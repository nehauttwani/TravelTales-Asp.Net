using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Travel_Agency___Data.ViewModels
{
    public class TravelPreferenceViewModel
    {
        public int PreferenceId { get; set; }
        public int CustomerId { get; set; }

        [Display(Name = "Preferred Climate")]
        public string? PreferredClimate { get; set; }

        [Display(Name = "Activities")]
        public List<string> SelectedActivities { get; set; } = new();
        public List<string> AvailableActivities { get; } = new()
    {
        "Hiking", "Beach", "Sightseeing", "Shopping",
        "Cultural Tours", "Adventure Sports", "Food Tours",
        "Wildlife", "Photography", "Historical Sites"
    };

        [Display(Name = "Travel Companion")]
        public string? TravelCompanion { get; set; }
        public List<string> TravelCompanionOptions { get; } = new()
    {
        "Solo", "Couple", "Family", "Friends", "Group"
    };

        [Display(Name = "Preferred Location")]
        public string? PreferredLocation { get; set; }
        public List<string> LocationOptions { get; } = new()
    {
        "City Center", "Beach Resort", "Mountains",
        "Countryside", "Historical District", "Wilderness"
    };

        public List<string> ClimateOptions { get; } = new()
    {
        "Tropical", "Mediterranean", "Desert",
        "Alpine", "Temperate", "Arctic"
    };
    }
}
