using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using System.Text.RegularExpressions;


namespace ITech.Security.Business
{
    //using NeinLinq;



    //public class FakeDbSet<TEntity> : DbSet<TEntity>, IQueryable<TEntity>, IDbAsyncEnumerable<TEntity>
    //    where TEntity : class
    //{
    //    private readonly IEnumerable<PropertyInfo> keys;
    //    private readonly ICollection<TEntity> items;
    //    private readonly IQueryable<TEntity> query;

    //    public FakeDbSet()
    //    {
    //        keys = typeof(TEntity).GetProperties()
    //            .Where(p => Attribute.IsDefined(p, typeof(KeyAttribute))
    //                        || "Id".Equals(p.Name, StringComparison.Ordinal))
    //            .ToList();

    //        items = new List<TEntity>();
    //        query = items.AsQueryable()
    //            .ToSubstitution(typeof(SqlFunctions), typeof(FakeDbFunctions))
    //            .ToNullsafe();
    //    }

    //    public override TEntity Add(TEntity entity)
    //    {
    //        if (entity == null)
    //            throw new ArgumentNullException("entity");
    //        if (keys.Any(k => k.GetValue(entity) == null))
    //            throw new ArgumentOutOfRangeException("entity");

    //        items.Add(entity);
    //        return entity;
    //    }

    //    public override IEnumerable<TEntity> AddRange(IEnumerable<TEntity> entities)
    //    {
    //        if (entities == null)
    //            throw new ArgumentNullException("entities");

    //        return entities.Select(Add).ToList();
    //    }

    //    public override TEntity Attach(TEntity entity)
    //    {
    //        if (entity == null)
    //            throw new ArgumentNullException("entity");
    //        if (keys.Any(k => k.GetValue(entity) == null))
    //            throw new ArgumentOutOfRangeException("entity");

    //        var item = items.SingleOrDefault(i =>
    //            keys.All(k => k.GetValue(entity).Equals(k.GetValue(i)))
    //        );

    //        if (item == null)
    //            items.Add(entity);
    //        return item ?? entity;
    //    }

    //    public override TDerivedEntity Create<TDerivedEntity>()
    //    {
    //        return Activator.CreateInstance<TDerivedEntity>();
    //    }

    //    public override TEntity Create()
    //    {
    //        return Activator.CreateInstance<TEntity>();
    //    }

    //    public override TEntity Find(params object[] keyValues)
    //    {
    //        if (keyValues == null)
    //            throw new ArgumentNullException("keyValues");
    //        if (keyValues.Any(k => k == null))
    //            throw new ArgumentOutOfRangeException("keyValues");
    //        if (keyValues.Length != keys.Count())
    //            throw new ArgumentOutOfRangeException("keyValues");

    //        return items.SingleOrDefault(i =>
    //            keys.Zip(keyValues, (k, v) => v.Equals(k.GetValue(i)))
    //                .All(r => r)
    //        );
    //    }

    //    public override Task<TEntity> FindAsync(params object[] keyValues)
    //    {
    //        return Task.FromResult(Find(keyValues));
    //    }

    //    public override Task<TEntity> FindAsync(CancellationToken cancellationToken, params object[] keyValues)
    //    {
    //        return Task.FromResult(Find(keyValues));
    //    }

    //    public override TEntity Remove(TEntity entity)
    //    {
    //        if (entity == null)
    //            throw new ArgumentNullException("entity");
    //        if (keys.Any(k => k.GetValue(entity) == null))
    //            throw new ArgumentOutOfRangeException("entity");

    //        var item = items.SingleOrDefault(i =>
    //            keys.All(k => k.GetValue(entity).Equals(k.GetValue(i)))
    //        );

    //        if (item != null)
    //            items.Remove(item);
    //        return item;
    //    }

    //    public override IEnumerable<TEntity> RemoveRange(IEnumerable<TEntity> entities)
    //    {
    //        if (entities == null)
    //            throw new ArgumentNullException("entities");

    //        return entities.Select(Remove).ToList();
    //    }

    //    public Type ElementType
    //    {
    //        get { return query.ElementType; }
    //    }

    //    public Expression Expression
    //    {
    //        get { return query.Expression; }
    //    }

    //    public IQueryProvider Provider
    //    {
    //        get { return query.Provider; }
    //    }

    //    IEnumerator IEnumerable.GetEnumerator()
    //    {
    //        return query.GetEnumerator();
    //    }

    //    public IEnumerator<TEntity> GetEnumerator()
    //    {
    //        return query.GetEnumerator();
    //    }

    //    IDbAsyncEnumerator IDbAsyncEnumerable.GetAsyncEnumerator()
    //    {
    //        return ((IDbAsyncEnumerable) query).GetAsyncEnumerator();
    //    }

    //    public IDbAsyncEnumerator<TEntity> GetAsyncEnumerator()
    //    {
    //        return ((IDbAsyncEnumerable<TEntity>) query).GetAsyncEnumerator();
    //    }
    //}

    //public static class FakeDbFunctions
    //{
    //    public static int? CharIndex(string toSearch, string target)
    //    {
    //        return CharIndex(toSearch, target, null);
    //    }

    //    public static int? CharIndex(string toSearch, string target, int? startLocation)
    //    {
    //        if (toSearch == null)
    //            return null;
    //        if (target == null)
    //            return null;

    //        return target.IndexOf(toSearch, (startLocation ?? 1) - 1, StringComparison.CurrentCultureIgnoreCase) + 1;
    //    }

    //    public static int? PatIndex(string textPattern, string target)
    //    {
    //        if (textPattern == null)
    //            return null;
    //        if (target == null)
    //            return null;

    //        var pattern = Regex.Escape(textPattern)
    //            .Replace("_", ".")
    //            .Replace("\\[.]", "_")
    //            .Replace("%", ".*")
    //            .Replace("\\[.*]", "%")
    //            .Replace("\\[\\[]", "\\[");

    //        var match = Regex.Match(target, "(?i)^" + pattern + "$");
    //        return match.Success ? match.Index + 1 : 0;
    //    }

    //    // TODO: implement more SQL functions
    //}
}