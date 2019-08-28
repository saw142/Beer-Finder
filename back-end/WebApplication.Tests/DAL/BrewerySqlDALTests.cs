using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using WebApplication.Web.DAL;
using System.Data.SqlClient;

namespace WebApplication.Tests.DAL
{
    [TestClass]
    public class BrewerySqlDALTests
    {
        

        [TestMethod]
        public void ShouldFindBreweries()
        {
            string search = "Platform";
            BrewerySqlDAL dal = new BrewerySqlDAL(@"Data Source=.\SQLEXPRESS;Initial Catalog=CleBrews;Integrated Security=True");
            var results = dal.SearchBreweries(search);

            Assert.IsTrue(results.BreweryResults.Count > 0);
        }

    }
}
