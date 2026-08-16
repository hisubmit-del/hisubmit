using HiSubmit.Application.Exceptions;
using HiSubmit.Application.Specifications.Base;
using HiSubmit.Domain.Contracts;
using HiSubmit.Client.SharedModels.Wrapper;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hisubmit.Client.SharedModels.Wrapper;
using System.Linq.Dynamic.Core;

namespace HiSubmit.Application.Extensions
{
    public static class QueryableExtensions
    {
        public static async Task<PaginatedResult<T>> ToPaginatedListAsync<T>
        (this IQueryable<T> source, int pageNumber, int pageSize, string[] orderBy, bool orderByAsc = false,
            bool getAllData = false) where T : class
        {
            if (source == null) throw new ApiException();
            pageNumber = pageNumber == 0 ? 1 : pageNumber;
            pageSize = pageSize == 0 ? 10 : pageSize;
            var count = await source.CountAsync();
            pageNumber = pageNumber <= 0 ? 1 : pageNumber;

            if (orderBy != null && orderBy.Length != 0 && !string.IsNullOrWhiteSpace(orderBy[0]))
            {
                var ordering = string.Join(',', orderBy);
                if (!orderByAsc)
                    ordering += " descending";
                source = source.OrderBy(ordering);
            }
            else
            {
                source = source.OrderBy("Id descending");
            }

            List<T> items;
            if (getAllData)
            {
                items = await source.ToListAsync();
            }
            else
            {
                items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            }

            return PaginatedResult<T>.Success(items, count, pageNumber, pageSize);
        }

        public static IQueryable<T> Specify<T>(this IQueryable<T> query,
            ISpecification<T> spec) where T : class, IEntity
        {
            var queryableResultWithIncludes = spec.Includes
                .Aggregate(query,
                    (current, include) => current.Include(include));
            var secondaryResult = spec.IncludeStrings
                .Aggregate(queryableResultWithIncludes,
                    (current, include) => current.Include(include));
            return secondaryResult.Where(spec.Criteria);
        }

        public static IQueryable<T> Specify<T>(this IQueryable<T> query,
            AndSpecification<T> spec) where T : class, IEntity
        {
            if (spec._Specifications.Any())
            {
                var includeQuery = query;
                IQueryable<T> q;
                IQueryable<T> includeString;
                foreach (var sp in spec._Specifications)
                {
                    includeQuery = sp.Includes
                        .Aggregate(includeQuery, (current, include) => current.Include(include));
                }

                includeString = includeQuery;
                foreach (var sp in spec._Specifications)
                {
                    includeString = sp.IncludeStrings
                        .Aggregate(includeString, (current, include) => current.Include(include));
                }

                q = includeString;
                foreach (var sp in spec._Specifications)
                {
                    q = q.Where(sp.Criteria);
                }

                return q;
            }

            var queryableResultWithIncludes = spec.Includes
                .Aggregate(query,
                    (current, include) => current.Include(include));
            var secondaryResult = spec.IncludeStrings
                .Aggregate(queryableResultWithIncludes,
                    (current, include) => current.Include(include));
            return secondaryResult.Where(spec._leftSpecification.Criteria).Where(spec._rightSpecification.Criteria);
        }


        public static async Task<PaginatedResult<T>> ToPaginatedListAsync<T>
            (this IQueryable<T> source, PagedRequest request) where T : class
        {
            return await source.ToPaginatedListAsync
            (request.PageNumber, request.PageSize, request.Orderby,
                request.OrderByAscending, request.GetAllData);
        }
    }
}