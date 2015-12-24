using System.Collections.Generic;

namespace LuceneSearch.Core.Model
{
    public class BookAPIResponse
    {
        public string Error { get; set; }
        public decimal Time { get; set; }
        public string Total { get; set; }
        public int Page { get; set; }
        public List<Book> Books { get; set; }

        public BookAPIResponse()
        {
            Books = new List<Book>();
        }
    }
}