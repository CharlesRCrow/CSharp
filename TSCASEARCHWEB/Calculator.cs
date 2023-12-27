namespace TSCASEARCHWEB
{
    public class Calculator
    {
        public static float DensityConverter(float density, string densityUnit) =>
            // needs to convert to kg/m 3
            densityUnit switch
            {
                "gPerCM" => density * 1000,
                "poundsGallons" => (float)(density * 119.826427),
                _ => density
            };

        public static float DensityReverter(float density, string densityUnit) =>
            // convert back to original units
            densityUnit switch
            {
                "gPerCM" => (float)(density * 0.001),
                "poundsGallons" => (float)(density * 0.00835),
                _ => density
            };

        // unit either celsius or fahrenheit
        public static float TempConverter(float temp, string unit) =>
            unit.Equals("celsius") ? temp : (temp - 32) * 5 / 9;

        // unit either liters or gallons
        public static float VolumeConverter(float volume, string unit) =>
            unit.Equals("liters") ? (float)(volume * 0.001) : (float)(volume * 0.00378541);

        // unit either kg or lbs
        public static float WeightConvert(float weight, string weightUnit) =>
            weightUnit.Equals("kg") ? weight : (float)(weight * 2.20462262);

        public static float Weight(float density, float volume) => density * volume;

        public static float AcidNeutralization(float batchWeight, float acidNumber, float finalAcidNumber,
            float molWeightNeut, float concNeut, ushort acidEquiv = 1, ushort baseEquiv = 1)
        {
            float equiv = (float)acidEquiv / (float)baseEquiv;
            float result = (float)((float)((acidNumber - finalAcidNumber) * batchWeight * molWeightNeut / 56105.67) / concNeut);

            return result * equiv;
        }
        public static float BaseNeutralization(float batchWeight, float initialBaseNumber, float finalBaseNumber,
            float molWeightNeut, float concNeut, ushort acidEquiv = 1, ushort baseEquiv = 1)
        {
            float equiv = (float)acidEquiv / (float)baseEquiv;
            float result = (float)((float)((initialBaseNumber - finalBaseNumber) * batchWeight * molWeightNeut / 56105.67) / concNeut);

            return result * equiv;
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
            float temp, float maxTemp) => density / (1 + thermalCoefficient * (maxTemp - temp));
    }
}