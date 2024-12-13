using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Travel_Agency___Data.Models;
using Travel_Agency___Data.Services;
using Travel_Agency___Data.ViewModels;

namespace Travel_Agency___Web.Controllers
{
    [Authorize]
    public class PreferencesController : Controller
    {
        private readonly TravelPreferenceService _preferenceService;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<PreferencesController> _logger;

        public PreferencesController(
            TravelPreferenceService preferenceService,
            UserManager<User> userManager,
            ILogger<PreferencesController> logger)
        {
            _preferenceService = preferenceService;
            _userManager = userManager;
            _logger = logger;
        }
        
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user?.CustomerId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var preferences = await _preferenceService.GetPreferencesAsync(user.CustomerId.Value);
            var viewModel = new TravelPreferenceViewModel
            {
                CustomerId = user.CustomerId.Value
            };

            if (preferences != null)
            {
                viewModel.PreferenceId = preferences.PreferenceId;
                viewModel.PreferredClimate = preferences.PreferredClimate;
                viewModel.SelectedActivities = preferences.Activities?.Split(',').ToList() ?? new List<string>();
                viewModel.TravelCompanion = preferences.TravelCompanion;
                viewModel.PreferredLocation = preferences.PreferredLocation;
            }

            return View(viewModel);
        }
        public async Task<IActionResult> Edit()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user?.CustomerId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var preferences = await _preferenceService.GetPreferencesAsync(user.CustomerId.Value);
            var viewModel = new TravelPreferenceViewModel
            {
                CustomerId = user.CustomerId.Value
            };

            if (preferences != null)
            {
                viewModel.PreferenceId = preferences.PreferenceId;
                viewModel.PreferredClimate = preferences.PreferredClimate;
                viewModel.SelectedActivities = preferences.Activities?.Split(',').ToList() ?? new List<string>();
                viewModel.TravelCompanion = preferences.TravelCompanion;
                viewModel.PreferredLocation = preferences.PreferredLocation;
            }

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(TravelPreferenceViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var preference = new TravelPreference
                {
                    CustomerId = viewModel.CustomerId,
                    PreferredClimate = viewModel.PreferredClimate,
                    Activities = string.Join(",", viewModel.SelectedActivities),
                    TravelCompanion = viewModel.TravelCompanion,
                    PreferredLocation = viewModel.PreferredLocation
                };

                await _preferenceService.UpdatePreferencesAsync(preference);
                TempData["SuccessMessage"] = "Travel preferences updated successfully!";
                return RedirectToAction("Profile", "Account");
            }

            return View(viewModel);
        }
    }
}
