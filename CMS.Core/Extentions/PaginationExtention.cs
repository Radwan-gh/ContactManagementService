using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CMS.Core.Extentions
{
    public static class PaginationExtention
    {
        public static List<T> ApplyPagination<T>(this IOrderedFindFluent<T, T> query, int page, int size, out int pageIndex, out long totaCount)
        {
            totaCount = query.CountDocuments();
            pageIndex = page;
            return query.Skip((page - 1) * size).Limit(size).ToList();
        }
        public static List<T> ApplyPagination<T>(this IOrderedQueryable<T> query, int page, int size, out int pageIndex, out int totaCount)
        {
            totaCount = query.Count();
            pageIndex = page;
            return query.Skip((page - 1) * size).Take(size).ToList();
        }

        // https://kevsoft.net/2020/01/27/paging-data-in-mongodb-with-csharp.html
        public static async Task<(long TotalCount, IReadOnlyList<TDocument> Data)> AggregateByPage<TDocument>(
            this IMongoCollection<TDocument> collection,
            FilterDefinition<TDocument> filterDefinition,
            SortDefinition<TDocument> sortDefinition,
            int skipSize,
            int pageSize,
            AggregateOptions options = null)
        {
            var countFacet = AggregateFacet.Create("count",
                PipelineDefinition<TDocument, AggregateCountResult>.Create(new[]
                {
                    PipelineStageDefinitionBuilder.Count<TDocument>()
                }));

            var dataFacet = AggregateFacet.Create("data",
                PipelineDefinition<TDocument, TDocument>.Create(new[]
                {
                    PipelineStageDefinitionBuilder.Sort(sortDefinition),
                    PipelineStageDefinitionBuilder.Skip<TDocument>(skipSize),
                    PipelineStageDefinitionBuilder.Limit<TDocument>(pageSize),
                }));

            var aggregation = await collection.Aggregate(options)
                .Match(filterDefinition)
                .Facet(countFacet, dataFacet)
                .ToListAsync();

            var count = aggregation.First()
                .Facets.First(x => x.Name == countFacet.Name)
                .Output<AggregateCountResult>()
                ?.FirstOrDefault()
                ?.Count;

            var data = aggregation.First()
                .Facets.First(x => x.Name == dataFacet.Name)
                .Output<TDocument>();

            return (count.GetValueOrDefault(), data);
        }

        public static async Task<(long TotalCount, IReadOnlyList<TOutput> Data)> AggregateByPage<TDocument, TOutput>(
            this IMongoCollection<TDocument> collection,
            FilterDefinition<TDocument> filterDefinition,
            SortDefinition<TDocument> sortDefinition,
            ProjectionDefinition<TDocument, TOutput> projection,
            int skipSize,
            int pageSize,
            AggregateOptions options = null)
        {
            var countFacet = AggregateFacet.Create("count",
                PipelineDefinition<TDocument, AggregateCountResult>.Create(new[]
                {
                    PipelineStageDefinitionBuilder.Count<TDocument>()
                }));

            var dataFacet = AggregateFacet.Create("data",
                PipelineDefinition<TDocument, TDocument>.Create(new[]
                {
                    PipelineStageDefinitionBuilder.Sort(sortDefinition),
                    PipelineStageDefinitionBuilder.Skip<TDocument>(skipSize),
                    PipelineStageDefinitionBuilder.Limit<TDocument>(pageSize),
                }).Project(projection));

            var aggregation = await collection.Aggregate(options)
                .Match(filterDefinition)
                .Facet(countFacet, dataFacet)
                .ToListAsync();

            var count = aggregation.First()
                .Facets.First(x => x.Name == countFacet.Name)
                .Output<AggregateCountResult>()
                ?.FirstOrDefault()
                ?.Count;

            var data = aggregation.First()
                .Facets.First(x => x.Name == dataFacet.Name)
                .Output<TOutput>();

            return (count.GetValueOrDefault(), data);
        }
    }
}
