using Domain;
using Domain.SearchCriteria;
using Exceptions;
using System.Collections.Generic;

namespace DataAccess.Repositories
{
    public class ReplacementRepository : IRepository<Replacement>
    {
        private List<Replacement> _replacements;
        private object _replacementsLocker;
        private static object _instanceLocker = new object();
        private static ReplacementRepository _instance;

        private ReplacementRepository()
        {
            _replacements = new List<Replacement>();
            _replacementsLocker = new object();
        }

        public static ReplacementRepository GetInstance()
        {
            lock (_instanceLocker)
            {
                if (_instance == null)
                    _instance = new ReplacementRepository();

                return _instance;
            }
        }

        public Replacement Get(int id)
        {
            lock (_replacementsLocker)
            {
                if (id < 0 || id >= _replacements.Count || _replacements[id] == null)
                    throw new ResourceNotFoundException("Specified replacement not found");

                return _replacements[id];
            }
        }

        public List<Replacement> GetBy(ISearchCriteria<Replacement> searchCriteria)
        {
            lock (_replacementsLocker)
            {
                return _replacements.FindAll(r => searchCriteria.Criteria(r));
            }
        }

        public List<Replacement> GetAll()
        {
            lock (_replacementsLocker)
            {
                return new List<Replacement>(_replacements);
            }
        }

        public int Store(Replacement replacement)
        {
            lock (_replacementsLocker)
            {
                Replacement newReplacement = new Replacement()
                {
                     Id = _replacements.Count,
                     Name = replacement.Name,
                     Brand = replacement.Brand,
                     Provider = replacement.Provider
                };
                _replacements.Add(newReplacement);

                return newReplacement.Id;
            }
        }

        public void Delete(int id)
        {
            Replacement repToDelete = Get(id);
            lock (_replacementsLocker)
            {
                _replacements.Remove(repToDelete);
            }
        }

        public Replacement Update(int id, Replacement replacementParams)
        {
            Replacement rep = Get(id);
            lock (_replacementsLocker)
            {
                rep.Name = replacementParams.Name;
                rep.Provider = replacementParams.Provider;
                rep.Brand = replacementParams.Brand;
            }

            return rep;
        }

        public int StoreNewCategory(int id, Category category)
        {
            Replacement rep = Get(id);
            lock (_replacementsLocker)
            {
                return rep.AddCategory(category);
            }
        }

        public void StorePhoto(int id, string path)
        {
            Replacement rep = Get(id);
            lock (_replacementsLocker)
            {
                rep.PhotoPath = path;
            }
        }

        public void RemovePhoto(int id)
        {
            Replacement rep = Get(id);
            lock (_replacementsLocker)
            {
                rep.PhotoPath = null;
            }
        }
    }
}
