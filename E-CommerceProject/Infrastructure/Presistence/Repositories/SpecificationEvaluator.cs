﻿using Domain.Contracts;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Repositories
{
    internal static class SpecificationEvaluator
    {
        public static IQueryable<T> GetQuery<T>
            (IQueryable<T> inputQuery , Specifications<T> specifications)
            where T: class
        {
            var query = inputQuery;

            if(specifications.Criteria is not null)
                query = query.Where (specifications.Criteria);

            query = specifications.IncludeExpressions.Aggregate(query, (currentQuery, includeExpression) => currentQuery
                    .Include(includeExpression));

            if (specifications.OrderBy is not null)
                query = query.OrderBy(specifications.OrderBy);

            else if(specifications.OrderByDescending is not null)
                query = query .OrderByDescending(specifications.OrderByDescending);

            return query;
        }
    }
}
