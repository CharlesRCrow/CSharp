using System.Reflection.Metadata.Ecma335;

namespace WeatherAPP
{
    public class Calculator
    {
        public float DensityConverter(float density, string unit)
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
        
        public float CelsiusConverter(float temp, string unit)
        {
            // needs to convert to celsius
            if (unit.Equals("celsius"))
            {
                return temp;
            }
            else
            {
                return (temp - 32) * 5 / 9;
            }
        }
        public float VolumeConverter(float volume, string unit)
        {
            // needs to convert to cubic meters
            if (unit.Equals("liters"))
            {
                return (float)(volume * 0.001);
            }
            else
            {
                return (float)(volume * 0.00378541);
            }
        }

        public float ThermalCoefficient(float densityOne, float densityTwo, 
            float tempOne, float tempTwo)
        {
            float volumeOne = 1 / densityOne;
            float volumeTwo = 1 / densityTwo;
                
            float changeVolume = Math.Abs(volumeOne - volumeTwo);
            float changeTemp = Math.Abs(tempOne - tempTwo);

            float thermalCoefficient = changeVolume / changeTemp / volumeOne;

            return thermalCoefficient;
        }

        public float DensityPrediction(float densityOne, float thermalCoefficient, 
            float tempOne, float maxTemp)
        {
            // predicts density at max storage temp
            float predictedDensity = densityOne / (1 + thermalCoefficient * (maxTemp - tempOne));
            return predictedDensity;
        }

        public float MaxWeight(float predictedDensity, float containerVolume)
        {
            return predictedDensity * containerVolume;
        }   
    }
}