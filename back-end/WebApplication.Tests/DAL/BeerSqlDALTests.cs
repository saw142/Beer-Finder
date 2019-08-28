using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using WebApplication.Web.DAL;
using System.Data.SqlClient;

namespace WebApplication.Tests.DAL
{
    [TestClass]
    public class BeerSqlDALTests
    {
        

        [TestMethod]
        public void ShouldFindBeers()
        {
            string search = "Ale";
            BeerSqlDAL dal = new BeerSqlDAL(@"Data Source=.\SQLEXPRESS;Initial Catalog=CleBrews;Integrated Security=True");
            var results = dal.SearchBeers(search);

            Assert.IsTrue(results.BeerResults.Count > 0);
;        }

    }
}
