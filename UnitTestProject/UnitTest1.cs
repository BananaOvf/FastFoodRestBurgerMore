using Microsoft.VisualStudio.TestTools.UnitTesting;
using FastFoodRest;
using System;
using System.Diagnostics.Eventing.Reader;
using FastFoodRest.View;
using System.Windows;
using System.Windows.Controls;

namespace UnitTestProject
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            // arrange
            var authorisationWindow = new AuthorisationWindow();
            string login = "admin";
            string password = "admin";


            // act
            PrivateObject privateObject = new PrivateObject(authorisationWindow);

            privateObject.SetField("login.Text", login);
            privateObject.SetField("password.Password", password);
            privateObject.Invoke("btn_Enter_Click", null, null);

            // assert
            foreach (Window window in Application.Current.Windows)
                if (window.GetType() == (new WorkWithCatalogWindow()).GetType())
                {
                    Assert.IsInstanceOfType(window.GetType(), (new WorkWithCatalogWindow()).GetType());
                    break;
                }
        }
    }
}
