using System.Linq;

namespace Taga.Core.Model.Searching
{
    public class SearchResult<T> where T : class, IEntity
    {
        public IQueryable<T> Results { get; set; }
        public PagingInfo PagingInfo { get; set; }

        private int? _totalCount;
        public int TotalCount
        {
            get
            {
                if (_totalCount == null)
                {
                    if (Results == null)
                        return 0;
                    _totalCount = Results.Count();
                }

                return _totalCount.Value;
            }
            set { _totalCount = value; }
        }

        public static SearchResult<T> Default(IQueryable<T> results)
        {
            return new SearchResult<T> { Results = results };
        }
    }
}
