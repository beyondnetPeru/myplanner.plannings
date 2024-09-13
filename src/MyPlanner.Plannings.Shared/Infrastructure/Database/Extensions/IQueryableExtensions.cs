﻿using MyPlanner.Plannings.Shared.Application.Dtos;

namespace MyPlanner.Plannings.Shared.Infrastructure.Database.Extensions
{
    public static class IQueryableExtensions
    {
        public static IQueryable<T> Paginate<T>(this IQueryable<T> queryable,
            PaginationDto pagination)
        {
            return queryable
                .Skip((pagination.Page - 1) * pagination.RecordsPerPage)
                .Take(pagination.RecordsPerPage);
        }
    }
}
