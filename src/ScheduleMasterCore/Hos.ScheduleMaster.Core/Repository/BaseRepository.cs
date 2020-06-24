using Hos.ScheduleMaster.Core.Common;
using Hos.ScheduleMaster.Core.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Hos.ScheduleMaster.Core.Repository
{
    public class BaseRepository<TModel> where TModel : class, IEntity
    {
        private readonly DbContext _db;

        private DbSet<TModel> DbSet => _db.Set<TModel>();

        public BaseRepository(SmDbContext context)
        {
            _db = context as DbContext;
        }

        public IQueryable<TModel> Table => this.DbSet;

        public IQueryable<TModel> TableNoTracking => this.DbSet.AsNoTracking();

        public void Add(TModel entity)
        {
            DbSet.Add(entity);
        }

        public void AddRange(IEnumerable<TModel> entity)
        {
            DbSet.AddRange(entity);
        }

        public void Delete(TModel entity)
        {
            DbSet.Attach(entity);
            DbSet.Remove(entity);
        }

        public void DeleteBy(Expression<Func<TModel, bool>> where)
        {
            var query = DbSet.Where(where);
            DeleteBy(query);
        }

        public void DeleteBy(IQueryable<TModel> query)
        {
            DbSet.RemoveRange(query.AsEnumerable());
        }

        public void Update(TModel entity, params Expression<Func<TModel, object>>[] modifiedPropertyNames)
        {
            var entry = _db.Entry(entity);
            DbSet.Attach(entity);
            if (modifiedPropertyNames.Length > 0)
            {
                foreach (var proName in modifiedPropertyNames)
                {
                    entry.Property(proName).IsModified = true;
                }
            }
        }

        public void Update(TModel entity, params string[] modifiedPropertyNames)
        {
            var entry = _db.Entry(entity);
            DbSet.Attach(entity);
            foreach (var proName in modifiedPropertyNames)
            {
                entry.Property(proName).IsModified = true;
            }
        }

        public void UpdateExcept(TModel entity, params Expression<Func<TModel, object>>[] exceptPropertyNames)
        {
            var entry = _db.Entry(entity);
            entry.State = EntityState.Modified;
            foreach (var proName in exceptPropertyNames)
            {
                entry.Property(proName).IsModified = false;
            }
        }

        public void Update(TModel entity)
        {
            var entry = _db.Entry(entity);
            entry.State = EntityState.Modified;
        }

        public void UpdateBy(Expression<Func<TModel, bool>> where, Expression<Func<TModel, object>> updater)
        {
            var updateMemberExpr = (MemberInitExpression)updater.Body;
            var updateMemberCollection = updateMemberExpr.Bindings.Cast<MemberAssignment>().Select(c =>
            new
            {
                c.Member.Name,
                Value = c.Expression.NodeType == ExpressionType.Constant ? ((ConstantExpression)c.Expression).Value : Expression.Lambda(c.Expression, null).Compile().DynamicInvoke()
            }).ToArray();
            string[] modifyPropertyNames = updateMemberCollection.Select(c => c.Name).ToArray();
            object[] modifyPropertyValues = updateMemberCollection.Select(c => c.Value).ToArray();

            UpdateBy(where, modifyPropertyNames, modifyPropertyValues);
        }

        public void UpdateBy(Expression<Func<TModel, bool>> where, object updater)
        {
            Type typeObj = updater.GetType();
            var modifyPropertyNames = typeObj.GetProperties().Select(m => m.Name).ToArray();
            List<object> values = new List<object>();
            foreach (var p in modifyPropertyNames)
            {
                values.Add(typeObj.GetProperty(p).GetValue(updater));
            }
            UpdateBy(where, modifyPropertyNames, values.ToArray());
        }

        public void UpdateBy(Expression<Func<TModel, bool>> where, string[] modifyPropertyNames, object[] modifyPropertyValues)
        {
            Type typeObj = typeof(TModel);
            List<PropertyInfo> listModifyProperty = new List<PropertyInfo>();
            foreach (var propertyName in modifyPropertyNames)
            {
                listModifyProperty.Add(typeObj.GetProperty(propertyName));
            }
            var listModifing = DbSet.Where(where);
            foreach (var item in listModifing)
            {
                for (int index = 0; index < listModifyProperty.Count; index++)
                {
                    PropertyInfo proModify = listModifyProperty[index];
                    proModify.SetValue(item, modifyPropertyValues[index], null);
                }
            }
        }

        public TModel FirstOrDefault(Expression<Func<TModel, bool>> where)
        {
            return DbSet.FirstOrDefault(where);
        }

        public async Task<TModel> FirstOrDefaultAsync(Expression<Func<TModel, bool>> where)
        {
            return await DbSet.FirstOrDefaultAsync(where);
        }

        public IQueryable<TModel> Where(Expression<Func<TModel, bool>> where)
        {
            return DbSet.Where(where);
        }

        public IQueryable<TModel> WhereNoTracking(Expression<Func<TModel, bool>> where)
        {
            return this.Where(where).AsNoTracking();
        }

        public int Count(Expression<Func<TModel, bool>> where)
        {
            return DbSet.Count(where);
        }
        public async Task<int> CountAsync(Expression<Func<TModel, bool>> where)
        {
            return await DbSet.CountAsync(where);
        }

        public bool Any(Expression<Func<TModel, bool>> where)
        {
            return DbSet.Any(where);
        }

        public async Task<bool> AnyAsync(Expression<Func<TModel, bool>> where)
        {
            return await DbSet.AnyAsync(where);
        }

        public IQueryable<TModel> WhereOrderBy<TOrderKey>(Expression<Func<TModel, bool>> where, Expression<Func<TModel, TOrderKey>> orderBy, bool isAsc = true)
        {
            if (isAsc)
            {
                return DbSet.Where(where).OrderBy(orderBy);
            }
            else
            {
                return DbSet.Where(where).OrderByDescending(orderBy);
            }
        }

        public IQueryable<TModel> WhereInclude<TOrderKey>(Expression<Func<TModel, bool>> where, Expression<Func<TModel, TOrderKey>> orderBy, bool isAsc = true, params string[] includeNames)
        {
            IQueryable<TModel> dbQuery = null;
            foreach (string includePropertyName in includeNames)
            {
                if (dbQuery == null)
                {
                    dbQuery = DbSet.Include(includePropertyName);
                }
                else
                {
                    dbQuery = dbQuery.Include(includePropertyName);
                }
            }
            if (isAsc)
            {
                return dbQuery.Where(where).OrderBy(orderBy);
            }
            else
            {
                return dbQuery.Where(where).OrderByDescending(orderBy);
            }
        }

        public ListPager<TModel> WherePager<TOrderKey>(
            ListPager<TModel> pager,
            Expression<Func<TModel, bool>> where,
            Expression<Func<TModel, TOrderKey>> orderBy,
            bool isAsc = true,
            bool queryTotalCount = true
            )
        {
            var query = Where(where);
            if (pager.Filters != null)
            {
                foreach (var filter in pager.Filters)
                {
                    query = query.Where(filter);
                }
            }
            var orderList = isAsc ? query.OrderBy(orderBy) : query.OrderByDescending(orderBy);
            pager.Rows = orderList.Skip(pager.SkipCount).Take(pager.PageSize).AsNoTracking().ToList();
            if (queryTotalCount) pager.Total = orderList.Count();
            return pager;
        }

        public IQueryable<TResult> Join<TInner, TKey, TResult>(IEnumerable<TInner> inner, Expression<Func<TModel, TKey>> outerKeySelector, Expression<Func<TInner, TKey>> innerKeySelector, Expression<Func<TModel, TInner, TResult>> resultSelector)
        {
            return DbSet.Join(inner, outerKeySelector, innerKeySelector, resultSelector);
        }



        public int ExecuteSql(string sql)
        {
            return _db.Database.ExecuteSqlRaw(sql);
        }

        public async Task<int> ExecuteSqlAsync(string sql)
        {
            return await _db.Database.ExecuteSqlRawAsync(sql);
        }

        public int ExecuteSql(string sql, List<DbParameter> spList)
        {
            return _db.Database.ExecuteSqlRaw(sql, spList.ToArray());
        }

        public async Task<int> ExecuteSqlAsync(string sql, List<DbParameter> spList)
        {
            return await _db.Database.ExecuteSqlRawAsync(sql, spList.ToArray());
        }

    }
}
