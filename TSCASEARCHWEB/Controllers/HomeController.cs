﻿using System.Diagnostics;
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
        // set default values
        weightUnit = weightUnit is null ? "kg" : weightUnit;
        equiv = equiv is null ? "" : equiv;
        baseEquiv = baseEquiv is null ? "" : baseEquiv;
        neutSelect = neutSelect is null ? "" : neutSelect;
        acidSelect = acidSelect is null ? "" : acidSelect;

        // set ViewData values so partially filled form's values are kept during reload
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

        // if form not properly filled out.  Return view
        if ((string.IsNullOrEmpty(weightBatch) && (string.IsNullOrEmpty(volumeValue) || string.IsNullOrEmpty(densityValue)))
            || string.IsNullOrEmpty(acid) || string.IsNullOrEmpty(molWeight) || string.IsNullOrEmpty(conc) || string.IsNullOrEmpty(equiv)
            || string.IsNullOrEmpty(baseEquiv) || string.IsNullOrEmpty(finalAcid))
        {
            return View();
        }

        // parse filled out data if data compatible
        float acidNumber = 0;
        float finalAcidNumber = 0;
        float molWeightNeut = 0;
        float concentration = 0;
        ushort equivalence = 0;
        ushort baseEquivalence = 0;
        float acidWeight = 0;
        float volumeSample = 0;
        float density = 0;

        bool parsed = float.TryParse(acid, out acidNumber) && float.TryParse(finalAcid, out finalAcidNumber) && float.TryParse(molWeight, out molWeightNeut) &&
                        float.TryParse(conc, out concentration) && ushort.TryParse(equiv, out equivalence) && ushort.TryParse(baseEquiv, out baseEquivalence);


        // if unsuccessfully parsed or if desired value higher than initial return error message to page
        if (finalAcidNumber > acidNumber || !parsed)
        {
            ViewData["error"] = "true";
            return View();
        }

        // convert percentage value
        concentration /= 100;

        // if weight not filled out calculate with density and volume if successfully parsed
        if (string.IsNullOrEmpty(weightBatch))
        {
            parsed = float.TryParse(densityValue, out density) && float.TryParse(volumeValue, out volumeSample);

            if (parsed) acidWeight = Calculator.Weight(density, volumeSample);
            else
            {
                ViewData["error"] = "true";
                return View();
            }
        }
        else
        {
            parsed = float.TryParse(weightBatch, out acidWeight);

            if (parsed)
            {
                // convert weight to kg if needed
                acidWeight = weightUnit.Equals("kg") ? acidWeight : (float)(acidWeight * 0.45359237);
            }
            else
            {
                ViewData["error"] = "true";
                return View();
            }

        }

        // gives weight of neutralizer to add
        float result = Calculator.AcidNeutralization(acidWeight, acidNumber, finalAcidNumber,
        molWeightNeut, concentration, equivalence, baseEquivalence);

        // convert kilograms to lbs if needed        
        result = Calculator.WeightConvert(result, weightUnit);

        // format results and return view
        ViewData["result"] = result.ToString("F02");

        ViewData["altWeight"] = (weightUnit.Equals("kg") ? result * 1000 : result * 16).ToString("N2");

        return View();
    }
    public IActionResult Base_Neutralization(string weightBatch, string weightUnit, string volumeValue,
        string volumeUnit, string densityValue, string densityUnit, string initialBase, string molWeight,
        string conc, string equiv, string acidEquiv, string neutSelect, string baseSelect, string finalBase)
    {

        // very similar to Acid_Neutralization function
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

        float baseNumber = 0;
        float finalBaseNumber = 0;
        float molWeightNeut = 0;
        float concentration = 0;
        ushort equivalence = 0;
        ushort acidEquivalence = 0;

        float baseWeight = 0;
        float volumeSample = 0;
        float density = 0;

        bool parsed = float.TryParse(initialBase, out baseNumber) && float.TryParse(finalBase, out finalBaseNumber) && float.TryParse(molWeight, out molWeightNeut) &&
                        float.TryParse(conc, out concentration) && ushort.TryParse(equiv, out equivalence) && ushort.TryParse(acidEquiv, out acidEquivalence);


        if (finalBaseNumber > baseNumber || !parsed)
        {
            ViewData["error"] = "true";
            return View();
        }

        concentration /= 100;

        if (string.IsNullOrEmpty(weightBatch))
        {
            parsed = float.TryParse(densityValue, out density) && float.TryParse(volumeValue, out volumeSample);

            if (parsed) baseWeight = Calculator.Weight(density, volumeSample);
            else
            {
                ViewData["error"] = "true";
                return View();
            }

        }
        else
        {
            parsed = float.TryParse(weightBatch, out baseWeight);

            if (parsed)
            {
                // convert weight to kg if needed
                baseWeight = weightUnit.Equals("kg") ? baseWeight : (float)(baseWeight * 0.45359237);
            }
            else
            {
                ViewData["error"] = "true";
                return View();
            }
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

        // set ViewData values so partially filled form's values are kept during reload 
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

        // if form not properly filled out.  Return view
        if (string.IsNullOrEmpty(firstDensity) || string.IsNullOrEmpty(secondDensity) || string.IsNullOrEmpty(firstTemp)
            || string.IsNullOrEmpty(secondDensity) || string.IsNullOrEmpty(containerSize) || string.IsNullOrEmpty(containerTemp))
        {
            return View();
        }

        // parse filled out data if data compatible
        float densityOne = 0;
        float densityTwo = 0;
        float tempOne = 0;
        float tempTwo = 0;
        float tempContainer = 0;
        float sizeContainer = 0;

        bool parsed = float.TryParse(firstDensity, out densityOne) && float.TryParse(secondDensity, out densityTwo) && float.TryParse(firstTemp, out tempOne)
                        && float.TryParse(secondTemp, out tempTwo) && float.TryParse(containerTemp, out tempContainer) && float.TryParse(containerSize, out sizeContainer);

        // if not parsed return error message to page
        if (!parsed)
        {
            ViewData["error"] = "true";
            return View();
        }

        // convert units for calculation 
        densityOne = Calculator.DensityConverter(densityOne, densityUnit);
        densityTwo = Calculator.DensityConverter(densityTwo, densityUnit);
        tempOne = Calculator.TempConverter(tempOne, tempUnit);
        tempTwo = Calculator.TempConverter(tempTwo, tempUnit);
        tempContainer = Calculator.TempConverter(tempContainer, tempUnit);
        sizeContainer = Calculator.VolumeConverter(sizeContainer, volumeUnit);

        // perform calculations using converted units
        float thermalCoefficient = Calculator.ThermalCoefficient(densityOne, densityTwo, tempOne, tempTwo);
        float predictedDensity = Calculator.DensityPrediction(densityOne, thermalCoefficient, tempOne, tempContainer);
        float maxWeight = Calculator.Weight(predictedDensity, sizeContainer);
        string weightUnit = volumeUnit.Equals("liters") ? "kg" : "lbs";

        // convert units back to original units and return results
        ViewData["Coefficient"] = thermalCoefficient.ToString("e4");
        ViewData["PredictedDensity"] = Calculator.DensityReverter(predictedDensity, densityUnit).ToString("F04");
        ViewData["MaxWeight"] = Calculator.WeightConvert(maxWeight, weightUnit).ToString("F02");

        // return warning if imputed density reading increased with corresponding temperature.  High likely hood data imputed is not valid.
        if ((tempOne > tempTwo && densityOne > densityTwo) || (tempTwo > tempOne && densityTwo > densityOne))
        {

            ViewData["warning"] = "true";
        }

        // return error message to view if calculated values not a number
        if (float.IsNaN(thermalCoefficient) || float.IsNaN(predictedDensity) || float.IsNaN(maxWeight))
        {
            ViewData["error"] = "true";
            return View();
        }

        // returns unit data to results section of page
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
        // trim unnecessary white space
        if (searchQuery is not null)
        {
            searchQuery = System.Text.RegularExpressions.Regex.Replace(searchQuery.Trim(), @"\s+", " ");
        }

        WeatherJSON model = new WeatherJSON();

        // create initial empty list of dictionaries
        List<Dictionary<string, string>> weatherList;

        ViewData["error"] = "false";
        ViewData["NoResults"] = "false";

        // if searchquery and weatherselect are not null return search results otherwise empty list;
        weatherList = (searchQuery is not null && weatherSelect is not null) ? model.WeatherGet(searchQuery, weatherSelect) : new List<Dictionary<string, string>>();

        if ((weatherList is null || weatherList.Count == 0) && searchQuery is null)
        {
            ViewData["searchLocation"] = "Submit Address for Weather Forecast";
            return View();
        }
        else if ((weatherList is null || weatherList.Count == 0) && searchQuery is not null)
        {
            ViewData["searchLocation"] = "No Results";
            return View();
        }
        else if (weatherSelect is null)
        {
            ViewData["searchLocation"] = "Seven Day Forecast : " + searchQuery;
        }
        else if (weatherSelect.Equals("seven"))
        {
            ViewData["searchLocation"] = "Seven Day Forecast : " + searchQuery;
        }
        else if (weatherSelect.Equals("hourly"))
        {
            ViewData["searchLocation"] = "Hourly Forecast : " + searchQuery;
        }

        // if error message in weatherList return error message to page
        if (weatherList is not null && weatherList[0].ContainsKey("Error"))
        {
            ViewData["searchLocation"] = "502 Bad Gateway : Connection Unsuccessful";

            ViewData["error"] = "true";
        }

        if (weatherList is not null && weatherList[0].ContainsKey("NoResults"))
        {
            ViewData["NoResults"] = "true";
        }

        ViewBag.MyList = weatherList;
        ViewData["weatherSelect"] = weatherSelect;
        ViewData["searchQuery"] = searchQuery;

        return View();
    }
    public IActionResult CASSearch(string searchQuery, string secondQuery, string isInactive = "off")
    {
        ViewData["noResults"] = "false";

        if (searchQuery is not null)
        {
            searchQuery = System.Text.RegularExpressions.Regex.Replace(searchQuery.Trim(), @"\s+", " ");
        }

        if (secondQuery is not null)
        {
            secondQuery = System.Text.RegularExpressions.Regex.Replace(secondQuery.Trim(), @"\s+", " ");
        }

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

                results = chemSearch.Union(numSearch!).AsQueryable();
            }

            ViewData["searchChem"] = "Results for: " + searchQuery;

            if (isInactive.Equals("on")) results = results.Where(p => p.Activity == "ACTIVE");

            if (secondQuery is not null)
            {
                ViewData["secondQuery"] = secondQuery;
                results = results.Where(p => EF.Functions.Like(p.ChemName, $"%{secondQuery}%"));
                ViewData["searchChem"] = "Results for: " + searchQuery + " & " + secondQuery;
            }

            if (!results.Any())
            {
                ViewData["noResults"] = "true";
                ViewData["searchChem"] = "No results for: " + searchQuery + " & " + secondQuery;
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