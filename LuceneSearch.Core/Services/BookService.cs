using System;
using System.Linq;
using System.Collections.Generic;
using LuceneSearch.Core.Model;
using LuceneSearch.Core.Services.ServiceModel;
using System.Diagnostics;
using System.IO;

namespace LuceneSearch.Core.Services
{
    public class BookService : IBookService
    {
        #region Private Members

        private const string WEB_CLIENT_SOURCE = "http://it-ebooks-api.info/";
        
        #endregion

        #region Public Methods

        public void AddBooksToIndex(string fileName)
        {
            List<Book> books = new List<Book>();
            using (StreamReader reader = new StreamReader(fileName))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var items = line.Split('#');
                    if(items.Count() == 6)
                    {
                        var book = new Book
                        {
                            Description = items[0].Trim(),
                            ID = Convert.ToInt64(items[1].Trim()),
                            Image = items[2].Trim(),
                            isbn = items[3].Trim(),
                            SubTitle = items[4].Trim(),
                            Title = items[5].Trim()
                        };
                        books.Add(book);
                    }
                }
            }
            
            LuceneHelper.IndexBooks(books);
        }

        public BookSearchResponse SearchBooks(string query)
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = LuceneHelper.Search(query);
            timer.Stop();
            result.SearchTime = timer.ElapsedMilliseconds.ToString();
            return result;
        }

        #endregion
    }
}