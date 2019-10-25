using Domain.Entitys.Base;
using Infra.Context;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Infra.DAOS.Base
{
    public abstract class CrudDAO<TEntity> where TEntity : Entity, new()
    {
        public TheMarvelContext Db { get; set; }
        
        public DbSet<TEntity> DbSet { get; set; }
        public CrudDAO( TheMarvelContext contextMarvel = null)
        {
           
            if (contextMarvel == null)
            {
               
                contextMarvel = new TheMarvelContext();
            }
            Db = contextMarvel;
            DbSet = Db.Set<TEntity>();
            
        }
        public virtual void SaveChanges()
        {
            try
            {
                Db.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Debug.WriteLine("Entidade do tipo \"{0}\" no estado \"{1}\" tem os seguintes erros de validação:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Debug.WriteLine("- Property: \"{0}\", Erro: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }
        }

        public virtual void Dispose()
        {
            Db.Dispose();
            GC.SuppressFinalize(this);
        }

        public virtual void DetachedObject(TEntity obj)
        {
            var attachedEntity = Db.ChangeTracker.Entries<TEntity>().FirstOrDefault(e => e.Entity.Id == obj.Id);
            if (attachedEntity != null)
            {
                Db.Entry<TEntity>(attachedEntity.Entity).State = EntityState.Detached;
                GC.Collect();
            }
        }

        public int Count(Expression<Func<TEntity, bool>> filtro)
        {
            if (filtro == null)
            {
                return DbSet.Count();
            }
            else
            {
                return DbSet.Where(filtro).Count();
            }
        }

        public virtual TEntity FindByIdWithNoTracking(int id)
        {
            return DbSet.AsNoTracking().SingleOrDefault(lp => lp.Id == id);
        }

        public virtual TEntity FindById(int id, bool tracking = true)
        {
            if (tracking)
            {
                return DbSet.FirstOrDefault(lp => lp.Id == id);
            }
            else
            {
                return DbSet.AsNoTracking().SingleOrDefault(lp => lp.Id == id);
            }

        }

        public virtual TEntity Save(TEntity obj)
        {
            if (obj.Id > 0)
            {
                return Update(obj);
            }
            var objAdd = DbSet.Add(obj);
            SaveChanges();
            return objAdd;
        }

        public virtual void SaveBach(IEnumerable<TEntity> objs)
        {
            foreach (var obj in objs)
            {
                obj.Id = 0;
                Save(obj);
            }
            SaveChanges();
        }

        public virtual TEntity Update(TEntity obj)
        {
            var entry = Db.Entry(obj);
            var attachedEntity = Db.ChangeTracker.Entries<TEntity>().FirstOrDefault(e => e.Entity.Id == obj.Id);
            if (attachedEntity != null)
            {
                Db.Entry<TEntity>(attachedEntity.Entity).State = EntityState.Modified;
            }
            else
            {
                DbSet.Attach(obj);
                entry.State = EntityState.Modified;
            }
            SaveChanges();
            return obj;
        }

        public virtual TEntity RemoveById(long id)
        {
            var obj = DbSet.Remove(DbSet.Find(id));
            SaveChanges();
            return obj;
        }

        public virtual void RemoveRange(int[] ids)
        {
            foreach (var id in ids)
            {
                DbSet.Remove(DbSet.Find(id));
            }
            SaveChanges();
        }

        public virtual void Remove(TEntity obj)
        {
            DbSet.Remove(obj);
            SaveChanges();
        }
    }
}
