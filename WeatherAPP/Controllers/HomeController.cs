using System.Runtime.InteropServices;
using System.IO;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WeatherAPP.Models;
using WeatherAPP.Views.Home;
using static WeatherAPP.Models.WeatherJSON;
using Microsoft.EntityFrameworkCore;

namespace WeatherAPP.Controllers;

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

    public IActionResult WeatherSearch(string searchQuery)
    {
        WeatherJSON model = new WeatherJSON();
        List<Dictionary<string, string>> weatherList;
        
        if (searchQuery is null)
        {
            weatherList = model.WeatherGet();
        }
        else
        {
            weatherList = model.WeatherGet(searchQuery);
        }
        
        ViewData["searchLocation"] = "Seven Day Forecast : " + searchQuery;
        ViewBag.MyList = weatherList;
        if (weatherList is null || weatherList.Count == 0)
        {
            ViewData["searchLocation"] = "Submit Address for Weather Forecast";
        }

        return View();
    }
    public IActionResult CASSearch(string searchQuery, string isInactive = "off")
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
            if (isInactive == "on")
            {
                results = results.Where(p => p.Activity == "ACTIVE");
            }
            
            ViewData["searchChem"] = "Results for: " + searchQuery;
            return View(results);
        }
    }        

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
