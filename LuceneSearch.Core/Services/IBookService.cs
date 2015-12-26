using LuceneSearch.Core.Model;
using LuceneSearch.Core.Services.ServiceModel;
using System.Collections.Generic;

namespace LuceneSearch.Core.Services
{
    public interface IBookService
    {
        /// <summary>
        ///  Adds index entries from excel file
        /// </summary>
        /// <param name="excelFile">The excel file.</param>
        void AddBooksToIndex(string csvFile);
        
        /// <summary>
        /// Searches the book index
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns></returns>
        BookSearchResponse SearchBooks(string query);
    }

}
