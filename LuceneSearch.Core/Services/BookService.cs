using System;
using System.Linq;
using System.Collections.Generic;
using LinqToExcel;
using LuceneSearch.Core.Model;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using LuceneSearch.Core.Services.ServiceModel;
using System.Diagnostics;

namespace LuceneSearch.Core.Services
{
    public class BookService : IBookService
    {
        #region Private Members

        private const string WEB_CLIENT_SOURCE = "http://it-ebooks-api.info/";
        
        #endregion

        #region Public Methods

        public void AddBooksToIndex(List<Book> books)
        {
            throw new NotImplementedException();
        }

        public void AddBooksFromExternalSource()
        {
            var books = GetExternalResponse().Result;
            LuceneHelper.IndexBooks(books.SelectMany(t => t.Books).ToList());
        }

        public void AddBooksToIndex(string excelFile)
        {
            const string sheetName = "Sheet1";

            ExcelQueryFactory excel = new ExcelQueryFactory(excelFile);
            var books = excel.Worksheet<Book>(sheetName).ToList();

            LuceneHelper.IndexBooks(books);
        }

        public BookSearchResponse SearchBooks(string query)
        {
            Stopwatch st = new Stopwatch();
            st.Start();
            var result = LuceneHelper.Search(query);
            st.Stop();
            result.SearchTime = st.ElapsedMilliseconds.ToString();
            return result;
        }

        #endregion

        #region Private Methods

        private async Task<List<BookAPIResponse>> GetExternalResponse()
        {
            List<BookAPIResponse> responses = new List<BookAPIResponse>();
            for (int i = 1; i <= 21; i++)
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(WEB_CLIENT_SOURCE);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = await client.GetAsync(string.Format("v1/search/computer/page/{0}", i));
                    if (response.IsSuccessStatusCode)
                    {
                        var book = await response.Content.ReadAsAsync<BookAPIResponse>();
                        responses.Add(book);
                    }
                }
            }
            return responses;
        }
        
        #endregion
    }
}