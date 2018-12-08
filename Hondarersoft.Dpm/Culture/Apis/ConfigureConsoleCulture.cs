using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
