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
    public IActionResult CASSearch(string searchQuery)
    {
        if (searchQuery is null)
        {
            return View();
        }
        else
        {
            IQueryable<Ca>? cas = db.Cas?.Where(p => EF.Functions.Like(p.ChemName, $"%{searchQuery}%"))
            // .Where(p => p.Activity == "ACTIVE")
            // .Where(p => p.Flag == "")
            .OrderBy(p => p.ChemName);
            // .Take(200); 

            return View(cas);
        }

        //     IQueryable<Ca>? cas = db.Cas?.Where(p => EF.Functions.Like(p.ChemName, $"%{searchQuery}%"))
        //     // .Where(p => p.Activity == "ACTIVE")
        //     // .Where(p => p.Flag == "")
        //     .OrderBy(p => p.ChemName);
        //     // .Take(200); 

        // return View(cas);
    }        

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
