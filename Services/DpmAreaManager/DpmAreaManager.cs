using Hondarersoft.Dpm.Areas;
using Hondarersoft.Dpm.ServiceProcess;
using System.Collections.Generic;

namespace Hondarersoft.Dpm.Services
{
    public class DpmAreaManager : AreaManagerCore
    {
        public static int Main(string[] args)
        {
            DpmAreaManager instance = new DpmAreaManager();

            #region Region for self install

            ServiceInstallParameter serviceInstallParameter = new ServiceInstallParameter
            {
                // DisplayName and Description are automatically obtained from AssemblyInfo.cs.
                ExecutableUsers = new List<string>() { "Everyone" }
            };

            if (instance.TryInstall(serviceInstallParameter) == true)
            {
                return instance.ExitCode;
            }

            #endregion

            Run(instance);

            return instance.ExitCode;
        }
    }

#if false
    class DpmAreaManager
    {
        internal struct TestStruct
        {
            public int intMember;
        }

        static void Main(string[] args)
        {

            if (Apis.Principal.IsAdministrator() != true)
            {
                Console.WriteLine("You are not Administrator.");

                AreaFactory.Instance.OpenArea("TestMemory", false);

                TestStruct _testStruct;

                AreaFactory.Instance.GetAccessor("TestMemory").Read(out _testStruct);
                Console.WriteLine($"_testStruct.intMember = {_testStruct.intMember}");
                _testStruct.intMember = 5678;
                AreaFactory.Instance.GetAccessor("TestMemory").Write(ref _testStruct);
                AreaFactory.Instance.GetAccessor("TestMemory").Read(out _testStruct);
                Console.WriteLine($"_testStruct.intMember = {_testStruct.intMember}");

                Console.ReadLine();

                return;
            }

            AreaFactory.Instance.CreateArea("TestMemory", 1, 1, typeof(TestStruct), true);

            TestStruct testStruct = new TestStruct
            {
                intMember = 1234
            };

            AreaFactory.Instance.GetAccessor("TestMemory").Write(ref testStruct);
            //AreaFactory.Instance.GetAccessor("TestMemory").Freeze();

            Console.WriteLine("created.");
            Console.ReadLine();
        }
    }
#endif
}
