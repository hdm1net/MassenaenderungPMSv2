namespace MassenaenderungPMSv2Gui.Helpers
{
    public class StringHelper
    {
        public static string LimitLength(string input, int maxLength)
        {
            if (string.IsNullOrEmpty(input) || maxLength < 3)
                return input;

            return input.Length <= maxLength
                ? input
                : input.Substring(0, maxLength - 3) + "...";
        }
    }
}
