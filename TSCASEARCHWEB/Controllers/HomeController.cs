using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TSCASEARCHWEB.Models;

namespace TSCASEARCHWEB.Controllers;

public class HomeController : Controller
{
    CasContext db = new CasContext();
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult About()
    {
        return View();
    }

    public IActionResult WeatherSearch(string searchQuery, string weatherSelect = "seven")
    {
        WeatherJSON model = new WeatherJSON();
        List<Dictionary<string, string>> weatherList;

        weatherList = (searchQuery is not null && weatherSelect is not null) ? model.WeatherGet(searchQuery, weatherSelect) : model.WeatherGet();

        if (weatherList is null || weatherList.Count == 0)
        {
            ViewData["searchLocation"] = "Submit Address for Weather Forecast";
        }
        else if (weatherSelect.Equals("seven"))
        {
            ViewData["searchLocation"] = "Seven Day Forecast : " + searchQuery;
        }
        else if (weatherSelect.Equals("hourly"))
        {
            ViewData["searchLocation"] = "Hourly Forecast : " + searchQuery;
        }

        ViewBag.MyList = weatherList;
        ViewData["weatherSelect"] = weatherSelect;
        ViewData["searchQuery"] = searchQuery;

        return View();
    }
    public IActionResult CASSearch(string searchQuery, string secondQuery, string isInactive = "off")
    {

        if (searchQuery is null || searchQuery.Length < 3)
        {
            return View();
        }
        else
        {
            ViewData["searchQuery"] = searchQuery;

            string digits = string.Concat(searchQuery.Where(Char.IsDigit));

            IQueryable<Ca>? chemSearch = db.Cas?.Where(p => EF.Functions.Like(p.ChemName, $"%{searchQuery}%"))
                .OrderBy(p => p.ChemName);

            IQueryable<Ca>? results = chemSearch;

            if (digits.Length > 2 && digits.Length < 11)
            {
                IQueryable<Ca>? numSearch = db.Cas?.Where(p => EF.Functions.Like(p.Casregno, $"%{digits}%"))
                    .OrderBy(p => p.ChemName);

                results = chemSearch.Union(numSearch).AsQueryable();
            }
            ViewData["searchChem"] = "Results for: " + searchQuery;
            if (isInactive.Equals("on"))
            {
                results = results.Where(p => p.Activity == "ACTIVE");
            }
            if (secondQuery is not null)
            {
                ViewData["secondQuery"] = secondQuery;
                results = results.Where(p => EF.Functions.Like(p.ChemName, $"%{secondQuery}%"));
                ViewData["searchChem"] = "Results for: " + searchQuery + " & " + secondQuery;
            }
            return View(results);
        }
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
