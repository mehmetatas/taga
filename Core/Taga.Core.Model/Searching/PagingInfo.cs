
namespace Taga.Core.Model.Searching
{
    public class PagingInfo
    {
        public PagingInfo()
        {
            PageSize = 10;
            PageIndex = 1;
        }

        public int PageSize { get; set; }
        public int PageIndex { get; set; } // not zero-based, first page: index = 1

        public int Start
        {
            get { return PageSize * (PageIndex - 1) + 1; }
        }

        public int End
        {
            get { return Start + PageSize; }
        }
    }
}
