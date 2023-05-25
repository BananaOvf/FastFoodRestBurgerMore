using Microsoft.VisualStudio.TestTools.UnitTesting;
using FastFoodRest;

namespace TestProject1
{
    [TestClass]
    public class UnitTest1
    {

        [TestMethod]
        public void TestMethod1()
        {
            Assert.AreEqual(App.adminPassword.ToString(), "admin");
        }
    }
}