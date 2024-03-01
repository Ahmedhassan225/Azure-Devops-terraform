namespace Domain.Shared
{
    public class BaseModel<T>
    {
        public BaseModel(T result, PageInfoAppModel pageInfo)
        {
            Result = result;
            PageInfo = pageInfo;
        }

        public BaseModel(T result)
        {
            Result = result;
        }

        public T Result { get; set; }
        public PageInfoAppModel PageInfo { get; set; }
    }

    public class PageInfoAppModel
    {
        public PageInfoAppModel()
        {
            CurrentPage = 1;
            TotalCount = 0;
            PageSize = 10;
        }

        public PageInfoAppModel(int currentPage, int totalCount, int pageSize)
        {
            CurrentPage = currentPage;
            TotalCount = totalCount;
            PageSize = pageSize;
        }

       public int CurrentPage { get; set; }
       public int TotalCount { get; set; }
       public int PageSize { get; set; }
    }

}
