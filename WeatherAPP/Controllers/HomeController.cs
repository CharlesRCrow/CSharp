using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WeatherAPP.Models;
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

    public IActionResult About()
    {
        return View();
    }
    
    public IActionResult Thermal_Expansion(string densityUnit, string tempUnit, string volumeUnit,
        string firstDensity, string firstTemp, string secondDensity, 
        string secondTemp, string containerSize, string containerTemp)
    {
        ViewData["firstDensity"] = firstDensity;
        ViewData["secondDensity"] = secondDensity;
        ViewData["firstTemp"] = firstTemp;
        ViewData["secondTemp"] = secondTemp;
        ViewData["containerSize"] = containerSize;
        ViewData["containerTemp"] = containerTemp;
        ViewData["densityUnit"] = densityUnit; 
        ViewData["volumeUnit"] = volumeUnit;
        ViewData["error"] = "false";                    
        
        if (string.IsNullOrEmpty(firstDensity) || string.IsNullOrEmpty(secondDensity) || string.IsNullOrEmpty(firstTemp)
            || string.IsNullOrEmpty(secondDensity) || string.IsNullOrEmpty(containerSize) || string.IsNullOrEmpty(containerTemp))
        {
            return View();
        }

        float densityOne = float.Parse(firstDensity);
        float densityTwo = float.Parse(secondDensity);

        densityOne = Calculator.DensityConverter(densityOne, densityUnit);
        densityTwo = Calculator.DensityConverter(float.Parse(secondDensity), densityUnit);
        float tempOne = Calculator.TempConverter(float.Parse(firstTemp), tempUnit);
        float tempTwo = Calculator.TempConverter(float.Parse(secondTemp), tempUnit);
        float tempContainer = Calculator.TempConverter(float.Parse(containerTemp), tempUnit);
        float sizeContainer = Calculator.VolumeConverter(float.Parse(containerSize), volumeUnit);

        float thermalCoefficient = Calculator.ThermalCoefficient(densityOne, densityTwo, tempOne, tempTwo);
        float predictedDensity = Calculator.DensityPrediction(densityOne, thermalCoefficient, tempOne, tempContainer);
        float maxWeight = Calculator.MaxWeight(predictedDensity, sizeContainer);
        string weightUnit = volumeUnit.Equals("liters") ? "kg" : "lbs";

        ViewData["Coefficient"] = thermalCoefficient.ToString("e4");
        ViewData["PredictedDensity"] = Calculator.DensityReverter(predictedDensity, densityUnit).ToString("F04");
        ViewData["MaxWeight"] = Calculator.WeightConvert(maxWeight, weightUnit).ToString("F02");
        
        if (float.IsNaN(thermalCoefficient) || float.IsNaN(predictedDensity) || float.IsNaN(maxWeight))
        {
            ViewData["error"] = "true";
            return View();
        }         

        if (densityUnit.Equals("kPerMeter"))
        {
            ViewData["densityUnit"] = "Kg/m³";
        }
        else if (densityUnit.Equals("gPerCM"))
        {
            ViewData["densityUnit"] = "g/cm³";
        }
        else
        {
            ViewData["densityUnit"] = "lbs/gallon";
        }

        if (tempUnit.Equals("celsius"))
        {
            ViewData["tempUnit"] = "℃";
        }
        else
        {
            ViewData["tempUnit"] = "°F";
        }
        
        ViewData["weightUnit"] = weightUnit;
        
        return View();
    }

    public IActionResult WeatherSearch(string searchQuery, string weatherSelect="seven")
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


