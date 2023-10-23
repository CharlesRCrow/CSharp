using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WeatherAPP.Models;
using WeatherAPP.Views.Home;

namespace WeatherAPP.Controllers;

public class HomeController : Controller
{
    //public Dictionary<string, string> location { get; set; }
    //public List<Dictionary<string, string>> weatherList { get; set; }
    
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

    public IActionResult WeatherSearch()
    {
        WeatherJSON model = new WeatherJSON();
       //ViewData["jsonDict"] = model.WeatherGet();
        var weatherList = model.WeatherGet() as List<Dictionary<string, string>>;
        ViewData["weatherList"] = weatherList;
        ViewBag.MyList = weatherList;
        return View();
    }    

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
