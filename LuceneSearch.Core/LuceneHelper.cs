#region Using

using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using LuceneSearch.Core.Model;
using LuceneSearch.Core.Services.ServiceModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

#endregion

namespace LuceneSearch.Core
{
    /// <summary>
    ///  Helper class responsible for interacting with the lucene.net
    ///  It provides static method for indexing and searching
    /// </summary>
    public static class LuceneHelper
    {
        #region Private Members

        private static string luceneDirectoryPath;
        private static FSDirectory _indexDirectory;
        private static FSDirectory IndexDirectory
        {
            get
            {
                // check if the current directory instance is null
                // if so create a new instance
                if (_indexDirectory == null)
                {
                    _indexDirectory = FSDirectory.Open(new DirectoryInfo(luceneDirectoryPath));
                }

                // There might be changes that the index is opened by another instance
                // of Index write. Check for lock and unlock the same
                if (IndexWriter.IsLocked(_indexDirectory))
                {
                    IndexWriter.Unlock(_indexDirectory);
                }
                string lockFilePath = Path.Combine(luceneDirectoryPath, "write.lock");
                if (File.Exists(lockFilePath))
                {
                    File.Delete(lockFilePath);
                }
                return _indexDirectory;
            }
        }
        
        #endregion

        #region Public Methods

        /// <summary>
        ///  Method to index list of books using Lucene.Net
        /// </summary>
        /// <param name="books"></param>
        public static void IndexBooks(List<Book> books)
        {
            var analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);
            using (var writer = new IndexWriter(IndexDirectory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED))
            {
                foreach (var book in books)
                {
                    AddEntryToIndex(book, writer);
                }
            }
            // close handles
            analyzer.Close();
        }

        /// <summary>
        ///  Method to optimize the index. This can be used depending the size of index.
        /// </summary>
        public static void Optimize()
        {
            var analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);
            using (var writer = new IndexWriter(IndexDirectory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED))
            {
                analyzer.Close();
                writer.Optimize();
                writer.Dispose();
            }
        }

        /// <summary>
        ///  Searches the lucene index for the given phrase/query
        /// </summary>
        /// <param name="searchQuery">phrase/qyery to search for</param>
        /// <returns>BookResponse service model</returns>
        public static BookSearchResponse Search(string searchQuery)
        {
            // validation
            if (string.IsNullOrEmpty(searchQuery.Replace("*", "").Replace("?", "")))
            {
                return new BookSearchResponse();
            }
            
            // set up lucene searcher
            using (var searcher = new IndexSearcher(IndexDirectory, false))
            {
                var hits_limit = 1000;
                var analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);
                
                var parser = new MultiFieldQueryParser(Lucene.Net.Util.Version.LUCENE_30, new[] { "Title", "Author", "SubTitle", "Description" }, analyzer);
                var query = parser.Parse(searchQuery);
                
                var hits = searcher.Search(query, null, hits_limit, Sort.RELEVANCE).ScoreDocs;
                var results = MapResults(hits, searcher);
                BookSearchResponse response = new BookSearchResponse
                {
                    TotalResults = results.Count,
                    Books = results
                };

                analyzer.Close();
                searcher.Dispose();
                return response;
            }
        }

        /// <summary>
        ///  Initializes the lucene index folder.
        ///  This must be called once during the application startup
        /// </summary>
        /// <param name="luceneDirectory"></param>
        public static void InitializeInstance(string luceneDirectory, bool resetIndex)
        {
            if (System.IO.Directory.Exists(luceneDirectory) && resetIndex)
            {
                System.IO.Directory.Delete(luceneDirectory, true);
            }

            System.IO.Directory.CreateDirectory(luceneDirectory);
            luceneDirectoryPath = luceneDirectory;
        }
        
        #endregion

        #region Private Methods

        private static void AddEntryToIndex(Book book, IndexWriter writer)
        {
            var searchQuery = new TermQuery(new Term("ID", book.Title.ToString()));
            writer.DeleteDocuments(searchQuery);

            // add new index entry
            var doc = new Document();

            // add lucene fields mapped to db fields
            doc.Add(new Field("Id", book.ID.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
            doc.Add(new Field("Title", string.IsNullOrEmpty(book.Title) ? string.Empty : book.Title, Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field("SubTitle",string.IsNullOrEmpty(book.SubTitle) ? string.Empty : book.SubTitle, Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field("Description",string.IsNullOrEmpty(book.Description) ? string.Empty : book.Description, Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field("ISBN", book.isbn, Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field("ImageN", book.Image, Field.Store.YES, Field.Index.ANALYZED));

            // add entry to index
            writer.AddDocument(doc);
        }

        

        private static List<Book> MapResults(ScoreDoc[] hits, IndexSearcher searcher)
        {
            return hits.Select(t => searcher.Doc(t.Doc)).Select(t => new Book
            {
                Title = t.Get("Title"),
                ID = Convert.ToInt64(t.Get("Id")),
                SubTitle = t.Get("SubTitle"),
                Image = t.Get("ImageN"),
                Description = t.Get("Description"),
                isbn = t.Get("ISBN")
            }).ToList();
        }

        
        
        #endregion
    }
}
