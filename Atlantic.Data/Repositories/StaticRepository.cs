using Atlantic.Data.Contexts;
using Atlantic.Data.Services;
using MongoDB.Entities;

namespace Atlantic.Data.Repositories
{
    public interface IStaticRepository<T> where T : Entity
    {
        IQueryable<T> Search();
        Task<IEnumerable<T>> GetAll();
        Task<T> GetById(string id);
        Task<T> Insert(T entity);
        Task<IList<T>> InsertRange(IList<T> entities);
        Task<T> Update(T entity);
        Task Delete(string id);
    }

    public class StaticRepository<T> : IStaticRepository<T> where T : Entity
    {
        protected readonly MongoContext _context;
        private readonly IHttpContextAccessorService _httpAccessor;
        //private DbSet<T> entities;
        string errorMessage = string.Empty;
        public StaticRepository(IHttpContextAccessorService httpAccessor)
        {
            _httpAccessor = httpAccessor;
            _context = new MongoContext(httpAccessor);
            //this.context = context;
            //entities = context.Set<T>();
        }
        public IQueryable<T> Search()
        {
            return _context.Queryable<T>();
        }
        public async Task<IEnumerable<T>> GetAll()
        {
            return await _context.Find<T>().ExecuteAsync();
        }
        public async Task<T> GetById(string id)
        {
            return await _context.Find<T>().OneAsync(id);
        }
        public async Task<T> Insert(T entity)
        {
            await _context.SaveAsync(entity);
            return entity;
            //context.SaveChanges();
        }
        public async Task<IList<T>> InsertRange(IList<T> entities)
        {
            await _context.SaveAsync(entities);
            return entities;
            //context.SaveChanges();
        }
        public async Task<T> Update(T entity)
        {
            //var _entity = await GetById(entity.ID);
            await _context.SaveAsync(entity);
            return entity;
        }
        public async Task Delete(string id)
        {
            var _entity = await GetById(id);
            await _context.SaveAsync(_entity);
        }


        // public Task<T> FirstOrDefault(Expression<Func<T, bool>> predicate)
        //     => context.Set<T>().FirstOrDefaultAsync(predicate);
    }
}
