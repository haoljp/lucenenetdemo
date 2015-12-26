using LuceneSearch.Core;
using LuceneSearch.Core.Services;
using System.Configuration;
using System.Web.Mvc;
using System.Web.Routing;

namespace LuceneSearch.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            string indexDirectory = Server.MapPath("~/Content/BookIndex"); 
            string csvFile = Server.MapPath("~/Content/CSVFile/book.csv");
            var bookService = new BookService();

            // you can comment bookService.AddBooksToIndex(csvFile); 
            // i.e., after the first run and also set resetIndex as false
            LuceneHelper.InitializeInstance(luceneDirectory: indexDirectory, resetIndex: true);
            bookService.AddBooksToIndex(csvFile);
        }
    }
}
