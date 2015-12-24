using LuceneSearch.Core;
using LuceneSearch.Core.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
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
            string indexDirectory = ConfigurationManager.AppSettings["LuceneIndexDirectory"];
            LuceneHelper.InitializeInstance(indexDirectory);
            //var bookService = new BookService();
            //bookService.AddBooksFromExternalSource();
            //bookService.OptimizeIndex();
        }
    }
}
