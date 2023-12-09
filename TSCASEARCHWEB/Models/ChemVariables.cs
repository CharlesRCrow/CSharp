namespace TSCASEARCHWEB.Models
{
    public class ChemVariables
    {
        public static string SwitchBaseNeutralizer(string input, string current)
        {
            string resultText = input switch
            {
                "sodiumHydroxide" => "39.9971",
                "potassiumHydroxide" => "56.11",
                "ammoniumHydroxide" => "35.04",
                _ => current,
            };
            return resultText;
        }
        public static string SwitchBaseEquiv(string input, string current)
        {
            string resultText = input switch
            {
                "sodiumHydroxide" => "1",
                "potassiumHydroxide" => "1",
                "ammoniumHydroxide" => "1",
                _ => current,
            };
            return resultText;
        }
        public static string SwitchAcidNeutralizer(string input, string current)
        {
            string resultText = input switch
            {
                "hcl" => "36.46",
                "acetic" => "60.052",
                "sulfuric" => "98.079",
                "carbonic" => "62.0248",
                "phosphoric" => "97.994",
                "ddbsa" => "326.49",
                _ => current,
            };
            return resultText;
        }
        public static string SwitchAcidEquiv(string input, string current)
        {
            string resultText = input switch
            {
                "hcl" => "1",
                "acetic" => "1",
                "ddbsa" => "1",
                "sulfuric" => "2",
                "carbonic" => "2",
                "phosphoric" => "3",
                _ => current,
            };
            return resultText;
        }
    }
}

