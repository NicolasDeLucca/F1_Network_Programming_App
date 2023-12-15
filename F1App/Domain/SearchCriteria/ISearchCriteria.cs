namespace Domain.SearchCriteria
{
    public interface ISearchCriteria<T>
    {
        bool Criteria(T entity);
    }
}
