using Hondarersoft.Dpm.Areas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AreaManagerCoreTest
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                AreaManagerService.Client.TestCommand("ABCDE");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Exception:\r\n{ex}");
            }
        }
    }
}
