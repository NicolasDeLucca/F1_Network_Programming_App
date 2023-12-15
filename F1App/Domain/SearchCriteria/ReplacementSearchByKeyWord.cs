namespace Domain.SearchCriteria
{
    public class ReplacementSearchByKeyWord : ISearchCriteria<Replacement>
    {
        public string KeyWord { get; set; }

        public ReplacementSearchByKeyWord(){ }

        public bool Criteria(Replacement replacement)
        {
            return replacement.Name == KeyWord || replacement.Provider == KeyWord ||
                replacement.Brand == KeyWord || replacement.Categories.Exists(c => c.Name == KeyWord);
        }
    }
}
