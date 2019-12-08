using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Hondarersoft.Dpm.Reflection;

namespace Hondarersoft.DpmTests.Reflection
{
    [TestClass]
    public class DelegatesTest
    {
        class MyClass
        {
            public int MyProperty { get; set; }

            public int myField = 8901;

            public int NullMyFunc()
            {
                Console.WriteLine("NullMyFunc");

                return 1234;
            }

            public int MyFunc(string message)
            {
                Console.WriteLine(message);

                return 1234;
            }

            public void NullMyAction()
            {
                Console.WriteLine("NullMyAction");
            }

            public void MyAction(string message)
            {
                Console.WriteLine(message);
            }
        }


        [TestMethod]
        public void TestMethod1()
        {
            object target = Delegates.CreateInstance(typeof(MyClass));

            Delegates.Set(target, "MyProperty", 1000);
            Console.WriteLine(Delegates.Get(target, "MyProperty"));

            Delegates.Set(target, "myField", 1234);
            Console.WriteLine(Delegates.Get(target, "myField"));

            Console.WriteLine(Delegates.Func(target, "NullMyFunc"));
            Console.WriteLine(Delegates.Func(target, "MyFunc", "MyFuncMessage"));

            Delegates.Action(target, "NullMyAction");
            Delegates.Action(target, "MyAction", "MyActionMessage");
        }
    }
}
