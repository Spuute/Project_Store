using Microsoft.VisualStudio.TestTools.UnitTesting;
using ButikProjektMedPatric;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.IO;
using System.Collections.Concurrent;
using System.Globalization;
using System.Windows;
using MainApplication;

namespace ButikProjektMedPatric.Tests
{
    [TestClass()]
    public class MainWindowTests
    {
        [TestMethod()]
        public void CountTotalProducts()
        {
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

            var result = MainWindow.FileDeserialisation(@"C:\Windows\Temp\PatricOchKevin\TestProducts.csv");

            Assert.AreEqual(7, result.Count);
        }

        [TestMethod()]
        public void FindPriceWithName()
        {
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;


            var result = MainWindow.FileDeserialisation(@"C:\Windows\Temp\PatricOchKevin\TestProducts.csv");

            foreach (KeyValuePair<Product, int> p in result)
            {
                if (p.Key.Name == "Bravo")
                {
                    Assert.AreEqual(29.90m, p.Key.Price);
                }



            }

        }
        [TestMethod()]
        public void FindDescriptionWithPrice()
        {
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;


            var result = MainWindow.FileDeserialisation(@"C:\Windows\Temp\PatricOchKevin\TestProducts.csv");

            foreach (KeyValuePair<Product, int> p in result)
            {
                if (p.Key.Price == 29.90m)
                {
                    Assert.AreEqual("Perfekt till frukost eller mellanmålet!", p.Key.Description);
                }



            }

        }
        [TestMethod()]
        public void FindImageWithDescription()
        {
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;


            var result = MainWindow.FileDeserialisation(@"C:\Windows\Temp\PatricOchKevin\TestProducts.csv");

            foreach (KeyValuePair<Product, int> p in result)
            {
                if (p.Key.Description == "Perfekt till frukost eller mellanmålet!")
                {
                    Assert.AreEqual("1.jpg", p.Key.Image);
                }

            }

        }



        [TestMethod()]
        public void DiscountCanBeUsed()
        {

            var result = MainWindow.TotalDiscounts(0);

            Assert.AreEqual("Discount code added", result);


        }

        [TestMethod()]
        public void DiscountCodeUsed()
        {

            var result = MainWindow.TotalDiscounts(1);

            Assert.AreEqual("You can only use one discount code", result);

        }
    }
}