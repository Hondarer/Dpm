using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Hondarersoft.Dpm.Apis.Tests
{
    [TestClass()]
    public class ReflectionTests
    {
        internal class TestClassA
        {
            public virtual void TestMethod() { }
        }

        internal class TestClassA_1 : TestClassA
        {
            public override void TestMethod() { }
        }

        internal class TestClassB
        {
            public virtual void TestMethod() { }
        }

        [TestMethod()]
        public void IsMethodInheritedTest()
        {
            Assert.AreEqual(true, Reflection.IsMethodInherited(new TestClassA_1(), typeof(TestClassA), nameof(TestClassA.TestMethod)));
            Assert.AreEqual(false, Reflection.IsMethodInherited(new TestClassA(), typeof(TestClassA), nameof(TestClassA.TestMethod)));
            Assert.AreEqual(false, Reflection.IsMethodInherited(new TestClassB(), typeof(TestClassB), nameof(TestClassB.TestMethod)));
        }
    }
}