using LuceneSearch.Core.Model;
using System.Collections.Generic;

namespace LuceneSearch.Core.Services.ServiceModel
{
    public class BookSearchResponse
    {
        public string SearchTime { get; set; }
        public List<Book> Books { get; set; }
        public int TotalResults { get; set; }
    }
}
