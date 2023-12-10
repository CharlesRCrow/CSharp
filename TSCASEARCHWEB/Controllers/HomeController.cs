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

    public IActionResult Acid_Neutralization(string weightBatch, string weightUnit, string volumeValue,
        string volumeUnit, string densityValue, string densityUnit, string acid, string molWeight,
        string conc, string equiv, string baseEquiv, string neutSelect, string acidSelect, string finalAcid)
    {
        weightUnit = weightUnit is null ? "kg" : weightUnit;
        equiv = equiv is null ? "" : equiv;
        baseEquiv = baseEquiv is null ? "" : baseEquiv;
        neutSelect = neutSelect is null ? "" : neutSelect;
        acidSelect = acidSelect is null ? "" : acidSelect;

        ViewData["weightBatch"] = weightBatch;
        ViewData["weightUnit"] = weightUnit;
        ViewData["volumeValue"] = volumeValue;
        ViewData["densityValue"] = densityValue;
        ViewData["acid"] = acid;
        ViewData["finalAcid"] = finalAcid;
        ViewData["molWeight"] = ChemVariables.SwitchBaseNeutralizer(neutSelect, molWeight);
        ViewData["conc"] = conc;
        ViewData["equiv"] = ChemVariables.SwitchAcidEquiv(acidSelect, equiv);
        ViewData["baseEquiv"] = ChemVariables.SwitchBaseEquiv(neutSelect, baseEquiv);
        ViewData["error"] = "false";
        ViewData["baseReadOnly"] = neutSelect.Equals("manualNeut") ? "" : "readonly";
        ViewData["acidReadOnly"] = acidSelect.Equals("manualAcid") ? "" : "readonly";

        if ((string.IsNullOrEmpty(weightBatch) && (string.IsNullOrEmpty(volumeValue) || string.IsNullOrEmpty(densityValue)))
            || string.IsNullOrEmpty(acid) || string.IsNullOrEmpty(molWeight) || string.IsNullOrEmpty(conc) || string.IsNullOrEmpty(equiv)
            || string.IsNullOrEmpty(baseEquiv) || string.IsNullOrEmpty(finalAcid))
        {
            return View();
        }

        float acidNumber = float.Parse(acid);
        float finalAcidNumber = float.Parse(finalAcid);
        float molWeightNeut = float.Parse(molWeight);
        float concentration = float.Parse(conc) / 100;
        ushort equivalence = ushort.Parse(equiv);
        ushort baseEquivalence = ushort.Parse(baseEquiv);
        float acidWeight;

        if (finalAcidNumber > acidNumber)
        {
            ViewData["error"] = "true";
            return View();
        }

        if (string.IsNullOrEmpty(weightBatch))
        {
            float density = Calculator.DensityConverter(float.Parse(densityValue), densityUnit);
            float volumeSample = Calculator.VolumeConverter(float.Parse(volumeValue), volumeUnit);
            acidWeight = Calculator.Weight(density, volumeSample);
        }
        else
        {
            acidWeight = float.Parse(weightBatch);
            // convert weight to kg if needed
            acidWeight = weightUnit.Equals("kg") ? acidWeight : (float)(acidWeight * 0.45359237);
        }

        // gives weight of neutralizer to add
        float result = Calculator.AcidNeutralization(acidWeight, acidNumber, finalAcidNumber,
        molWeightNeut, concentration, equivalence, baseEquivalence);

        // convert kilograms to lbs if needed        
        result = Calculator.WeightConvert(result, weightUnit);


        ViewData["result"] = result.ToString("F02");

        ViewData["altWeight"] = (weightUnit.Equals("kg") ? result * 1000 : result * 16).ToString("N2");

        return View();
    }
    public IActionResult Base_Neutralization(string weightBatch, string weightUnit, string volumeValue,
        string volumeUnit, string densityValue, string densityUnit, string initialBase, string molWeight,
        string conc, string equiv, string acidEquiv, string neutSelect, string baseSelect, string finalBase)
    {
        weightUnit = weightUnit is null ? "kg" : weightUnit;
        equiv = equiv is null ? "" : equiv;
        acidEquiv = acidEquiv is null ? "" : acidEquiv;
        neutSelect = neutSelect is null ? "" : neutSelect;
        baseSelect = baseSelect is null ? "" : baseSelect;

        ViewData["weightBatch"] = weightBatch;
        ViewData["weightUnit"] = weightUnit;
        ViewData["volumeValue"] = volumeValue;
        ViewData["densityValue"] = densityValue;
        ViewData["initialBase"] = initialBase;
        ViewData["finalBase"] = finalBase;
        ViewData["molWeight"] = ChemVariables.SwitchAcidNeutralizer(neutSelect, molWeight);
        ViewData["conc"] = conc;
        ViewData["baseEquiv"] = ChemVariables.SwitchBaseEquiv(baseSelect, equiv);
        ViewData["acidEquiv"] = ChemVariables.SwitchAcidEquiv(neutSelect, acidEquiv);
        ViewData["error"] = "false";
        ViewData["acidReadOnly"] = neutSelect.Equals("manualAcid") ? "" : "readonly";
        ViewData["baseReadOnly"] = baseSelect.Equals("manualBase") ? "" : "readonly";

        if ((string.IsNullOrEmpty(weightBatch) && (string.IsNullOrEmpty(volumeValue) || string.IsNullOrEmpty(densityValue)))
            || string.IsNullOrEmpty(initialBase) || string.IsNullOrEmpty(molWeight) || string.IsNullOrEmpty(conc) || string.IsNullOrEmpty(equiv)
            || string.IsNullOrEmpty(acidEquiv) || string.IsNullOrEmpty(finalBase))
        {
            return View();
        }

        float baseNumber = float.Parse(initialBase);
        float finalBaseNumber = float.Parse(finalBase);
        float molWeightNeut = float.Parse(molWeight);
        float concentration = float.Parse(conc) / 100;
        ushort equivalence = ushort.Parse(equiv);
        ushort acidEquivalence = ushort.Parse(acidEquiv);
        float baseWeight;

        if (finalBaseNumber > baseNumber)
        {
            ViewData["error"] = "true";
            return View();
        }

        if (string.IsNullOrEmpty(weightBatch))
        {
            float density = Calculator.DensityConverter(float.Parse(densityValue), densityUnit);
            float volumeSample = Calculator.VolumeConverter(float.Parse(volumeValue), volumeUnit);
            baseWeight = Calculator.Weight(density, volumeSample);
        }
        else
        {
            baseWeight = float.Parse(weightBatch);
            // convert weight to kg if needed
            baseWeight = weightUnit.Equals("kg") ? baseWeight : (float)(baseWeight * 0.45359237);
        }

        // gives weight of neutralizer to add
        float result = Calculator.BaseNeutralization(baseWeight, baseNumber, finalBaseNumber,
        molWeightNeut, concentration, equivalence, acidEquivalence);

        // convert kilograms to lbs if needed        
        result = Calculator.WeightConvert(result, weightUnit);

        ViewData["result"] = result.ToString("F02");

        ViewData["altWeight"] = (weightUnit.Equals("kg") ? result * 1000 : result * 16).ToString("N2");

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
        ViewData["warning"] = "false";

        if (string.IsNullOrEmpty(firstDensity) || string.IsNullOrEmpty(secondDensity) || string.IsNullOrEmpty(firstTemp)
            || string.IsNullOrEmpty(secondDensity) || string.IsNullOrEmpty(containerSize) || string.IsNullOrEmpty(containerTemp))
        {
            return View();
        }

        float densityOne = Calculator.DensityConverter(float.Parse(firstDensity), densityUnit);
        float densityTwo = Calculator.DensityConverter(float.Parse(secondDensity), densityUnit);
        float tempOne = Calculator.TempConverter(float.Parse(firstTemp), tempUnit);
        float tempTwo = Calculator.TempConverter(float.Parse(secondTemp), tempUnit);
        float tempContainer = Calculator.TempConverter(float.Parse(containerTemp), tempUnit);
        float sizeContainer = Calculator.VolumeConverter(float.Parse(containerSize), volumeUnit);

        float thermalCoefficient = Calculator.ThermalCoefficient(densityOne, densityTwo, tempOne, tempTwo);
        float predictedDensity = Calculator.DensityPrediction(densityOne, thermalCoefficient, tempOne, tempContainer);
        float maxWeight = Calculator.Weight(predictedDensity, sizeContainer);
        string weightUnit = volumeUnit.Equals("liters") ? "kg" : "lbs";

        ViewData["Coefficient"] = thermalCoefficient.ToString("e4");
        ViewData["PredictedDensity"] = Calculator.DensityReverter(predictedDensity, densityUnit).ToString("F04");
        ViewData["MaxWeight"] = Calculator.WeightConvert(maxWeight, weightUnit).ToString("F02");

        if ((tempOne > tempTwo && densityOne > densityTwo) || (tempTwo > tempOne && densityTwo > densityOne))
        {

            ViewData["warning"] = "true";
        }

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

    public IActionResult WeatherSearch(string searchQuery, string weatherSelect = "seven")
    {
        WeatherJSON model = new WeatherJSON();
        List<Dictionary<string, string>> weatherList;

        ViewData["hourlyChecked"] = "";
        ViewData["sevenChecked"] = "";

        

        weatherList = (searchQuery is not null && weatherSelect is not null) ? model.WeatherGet(searchQuery, weatherSelect) : model.WeatherGet();

        if (weatherList is null || weatherList.Count == 0)
        {
            ViewData["searchLocation"] = "Submit Address for Weather Forecast";
        }
        else if (weatherSelect is null)
        {
            ViewData["searchLocation"] = "Seven Day Forecast : " + searchQuery;
        }
        else if (weatherSelect.Equals("seven"))
        {
            ViewData["sevenChecked"] = "checked";
            ViewData["searchLocation"] = "Seven Day Forecast : " + searchQuery;
        }
        else if (weatherSelect.Equals("hourly"))
        {
            ViewData["hourlyChecked"] = "checked";
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

            if (chemSearch is null)
            {
                return View();
            }
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