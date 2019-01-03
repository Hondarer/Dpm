using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;

namespace Hondarersoft.Dpm.Apis.Tests
{
    [TestClass()]
    public class ServiceProcessTests
    {
        [TestMethod()]
        public void IsServiceSessionTest()
        {
            // Since we never run tests from the service session, we can not test all the paths.
            if (Process.GetCurrentProcess().SessionId == 0)
            {
                Assert.AreEqual(true, ServiceProcess.IsServiceSession());
            }
            else
            {
                Assert.AreEqual(false, ServiceProcess.IsServiceSession());
            }
        }
    }
}