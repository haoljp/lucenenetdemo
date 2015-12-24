using LuceneSearch.Core.Services;
using System.Web.Mvc;

namespace LuceneSearch.Web.Controllers
{
    public class SearchController : Controller
    {
        #region Private Members

        private readonly IBookService _bookService;
        
        #endregion

        #region Ctor

        public SearchController()
        {
            _bookService = new BookService();
        }
        
        #endregion

        #region Actions

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SearchBooks(string searchTerm)
        {
            var books = _bookService.SearchBooks(searchTerm);
            return PartialView("_SearchResults", books);
        } 

        #endregion
	}
}