using System.Reflection.Metadata.Ecma335;

namespace WeatherAPP
{
    public class Calculator
    {
        public static float DensityConverter(float density, string unit)
        {
            // needs to convert to kg/m 3 
            if (unit.Equals("gPerCM"))
            {
                return density * 1000;
            }
            else if (unit.Equals("kPerMeter"))
            {
                return density;
            }
            else if (unit.Equals("poundsGallons"))
            {
                return (float)(density * 119.826427);
            }
            return density;
        }
        
        public static float TempConverter(float temp, string unit)
        {
            // needs to convert to celsius
            if (unit.Equals("celsius"))
            {
                return temp;
            }
            else
            {
                // only other option is fahrenheit
                return (temp - 32) * 5 / 9;
            }
        }
        public static float VolumeConverter(float volume, string unit)
        {
            // needs to convert to cubic meters
            if (unit.Equals("liters"))
            {
                return (float)(volume * 0.001);
            }
            else 
            {
                // only other option is gallons
                return (float)(volume * 0.00378541);
            }
        }

        public static float ThermalCoefficient(float densityOne, float densityTwo, 
            float tempOne, float tempTwo)
        {
            float volumeOne = 1 / densityOne;
            float volumeTwo = 1 / densityTwo;
                
            float changeVolume = Math.Abs(volumeOne - volumeTwo);
            float changeTemp = Math.Abs(tempOne - tempTwo);

            float thermalCoefficient = changeVolume / changeTemp / volumeOne;

            return thermalCoefficient;
        }

        public static float DensityPrediction(float density, float thermalCoefficient, 
            float temp, float maxTemp)
        {
            // predicts density at max storage temp
            float predictedDensity = density / (1 + thermalCoefficient * (maxTemp - temp));
            return predictedDensity;
        }

        public static float MaxWeight(float predictedDensity, float containerVolume)
        {
            return predictedDensity * containerVolume;
        }

        public static float WeightConvert(float weight, string weightUnit)
        {
            if (weightUnit.Equals("kg"))
            {
                // weight already in correct unit
                return weight;
            }
            else
            {
                // only other option is pounds
                return (float)(weight * 2.20462262);
            }
        } 

        public static float DensityReverter(float density, string densityUnit)
        {
            if (densityUnit.Equals("kPerMeter"))
            {
                // density already in correct unit
                return density;
            }
            else if (densityUnit.Equals("gPerCM"))
            {
                return (float)(density * 0.001);
            }
            else
            {
                // only other option is pounds/gallon
                return (float)(density * 0.00835);
            }
        }
        public static float AcidNeutralization(float batchWeight, float acidNumber, 
            float molWeightNeut = (float) 56.10567, float concNeut = (float) 0.45)
        {
            float result = (float)(batchWeight * acidNumber * molWeightNeut / 56105.67 / concNeut);
            return result;
        }  
    }
}