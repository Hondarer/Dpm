using System;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Hondarersoft.Dpm;

namespace Hondarersoft.Dpm
{
    class AreaManager
    {
        internal struct TestStruct
        {
            public int intMember;
        }

        static void Main(string[] args)
        {
            if (Apis.IsAdministrator() != true)
            {
                Console.WriteLine("You are not Administrator.");

#if true
                try
                {
                    bool _createNew;
                    Mutex _mutex = new Mutex(false, @"Global\TestMutex", out _createNew);
                    if (_createNew == false)
                    {
                        Console.WriteLine("Found mutex");
                        if (_mutex.WaitOne(60 * 1000) == true)
                        {
                            Console.WriteLine("Take mutex");
                            _mutex.ReleaseMutex();
                        }
                    }
                    else
                    {
                        _mutex.Close();
                        _mutex.Dispose();
                        throw new IOException($"A mutex named 'HOGE' not exist.");
                    }
                }
                catch
                {
                    // 取得失敗
                    throw;
                }
#endif

                if (Apis.MemoryMappedFileExists(@"Global\Hondarersoft.Dpm.TestMemory") != true)
                {
                    Console.WriteLine("MemoryMappedFile not exists.");
                    Console.ReadLine();
                    return;
                }

#if false
                // 読み取り専用で開く(アクセサーの書き込み操作は例外となる)
                MemoryMappedFile _mmf = MemoryMappedFile.OpenExisting(@"Global\Hondarersoft.Dpm.TestMemory", MemoryMappedFileRights.Read);
                MemoryMappedViewAccessor _accesser = _mmf.CreateViewAccessor(0, 1024, MemoryMappedFileAccess.Read);
#else
                // 読み書き可能で開く
                MemoryMappedFile _mmf = MemoryMappedFile.OpenExisting(@"Global\Hondarersoft.Dpm.TestMemory", MemoryMappedFileRights.ReadWrite);
                MemoryMappedViewAccessor _accesser = _mmf.CreateViewAccessor(0, 1024, MemoryMappedFileAccess.ReadWrite);
#endif

                TestStruct _testStruct;

                _accesser.Read(0, out _testStruct);
                Console.WriteLine($"_testStruct.intMember = {_testStruct.intMember}");
                _testStruct.intMember = 5678;
                _accesser.Write(0, ref _testStruct);
                _accesser.Read(0, out _testStruct);
                Console.WriteLine($"_testStruct.intMember = {_testStruct.intMember}");

                Console.ReadLine();

                _mmf.Dispose();

                return;
            }

#if true
            MutexSecurity mutexSecurity = new MutexSecurity();
            mutexSecurity.AddAccessRule(new MutexAccessRule("everyone", MutexRights.Synchronize | MutexRights.Modify, AccessControlType.Allow));
            bool createNew;
            try
            {
                Mutex mutex = new Mutex(false, @"Global\TestMutex", out createNew, mutexSecurity);
                if (createNew == true)
                {
                    if (mutex.WaitOne(1000) == true)
                    {
                        Console.WriteLine("Take mutex");
                        Console.ReadLine();
                        mutex.ReleaseMutex();
                    }
                }
                else
                {
                    // 所有者になれなかった
                    mutex.Close();
                    mutex.Dispose();
                    throw new IOException($"A mutex named 'HOGE' already existed.");
                }
            }
            catch
            {
                // 取得失敗
                throw;
            }
#endif

            MemoryMappedFileSecurity customSecurity = new MemoryMappedFileSecurity();
            customSecurity.AddAccessRule(new AccessRule<MemoryMappedFileRights>("everyone", MemoryMappedFileRights.ReadWrite, System.Security.AccessControl.AccessControlType.Allow));
            MemoryMappedFile mmf = MemoryMappedFile.CreateOrOpen(@"Global\Hondarersoft.Dpm.TestMemory", 1024, MemoryMappedFileAccess.ReadWrite, MemoryMappedFileOptions.None, customSecurity, System.IO.HandleInheritability.None);
            MemoryMappedViewAccessor accesser = mmf.CreateViewAccessor();

            TestStruct testStruct = new TestStruct
            {
                intMember = 1234
            };

            accesser.Write(0, ref testStruct);

            Console.WriteLine("created.");
            Console.ReadLine();

            accesser.Dispose();
            mmf.Dispose();
        }
    }
}
