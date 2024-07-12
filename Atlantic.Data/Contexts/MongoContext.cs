using MongoDB.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atlantic.Data.Audit;
using Atlantic.Data.Services;

namespace Atlantic.Data.Contexts
{
    public class MongoContext : DBContext
    {
        private readonly IHttpContextAccessorService _httpAccessor;
        public MongoContext(IHttpContextAccessorService httpAccessor)
        {
            _httpAccessor = httpAccessor;
        }
        public MongoContext()
        {
        }
        protected override Action<T> OnBeforeSave<T>()
        {
            var type = typeof(T).IsSubclassOf(typeof(AuditableEntitySerial));
            Action<AuditableEntitySerial> action = f =>
            {
                if (f.ID == null)
                {
                    f.CreatedById = _httpAccessor?.CurrentUserId();
                    f.CreatedByNme = _httpAccessor?.CurrentUsername();
                    f.CreatedDate = DateTime.UtcNow;
                }
                //else
                //{
                    f.ModifiedById = _httpAccessor?.CurrentUserId();
                    f.ModifiedByName = _httpAccessor?.CurrentUsername();
                    f.ModifiedDate = DateTime.UtcNow;
                //}
            };

            return action as Action<T>;
        }

        protected override Action<UpdateBase<T>> OnBeforeUpdate<T>()
        {
            var type = typeof(T).IsSubclassOf(typeof(AuditableEntitySerial));
            Action<AuditableEntitySerial> action = f =>
            {
                if (f.ID == null)
                {
                    f.CreatedById = _httpAccessor.CurrentUserId();
                    f.CreatedByNme = _httpAccessor.CurrentUsername();
                    f.CreatedDate = DateTime.UtcNow;
                }
                //else
                //{
                    f.ModifiedById = _httpAccessor.CurrentUserId();
                    f.ModifiedByName = _httpAccessor.CurrentUsername();
                    f.ModifiedDate = DateTime.UtcNow;
                //}
            };
            return action as Action<UpdateBase<T>>;
        }
    }
}
