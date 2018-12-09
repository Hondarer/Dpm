﻿using System;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Hondarersoft.Dpm;
using Hondarersoft.Dpm.Areas;

namespace Hondarersoft.Dpm
{
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
}
