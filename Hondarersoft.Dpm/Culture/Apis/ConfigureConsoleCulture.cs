using System;
using System.Globalization;
using System.Text;

namespace Hondarersoft.Dpm.Apis
{
    public static partial class Culture
    {
        public static void ConfigureConsoleCulture()
        {
            if (Console.OutputEncoding == Encoding.GetEncoding("shift_jis")) // 932
            {
                CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("ja");
            }
            else
            {
                CultureInfo.CurrentUICulture = CultureInfo.InvariantCulture;
            }
        }
    }
}
