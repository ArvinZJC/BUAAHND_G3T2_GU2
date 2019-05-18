// csharp file that provides a paginated list for the pagination

#region Using Directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
#endregion Using Directives

namespace NewEraFlowerStore.Extensions
{
    /// <summary>
    /// Extending from the class <see cref="List{T}"/>, the class <see cref="PaginatedList{T}"/> represents a strongly typed paginated list of objects for the pagination.
    /// </summary>
    /// <typeparam name="T">a specified variable type</typeparam>
    public class PaginatedList<T> : List<T>
    {
        /// <summary>
        /// A specified page index.
        /// </summary>
        public int PageIndex { get; private set; }
        /// <summary>
        /// The total number of pages.
        /// </summary>
        public int TotalPages { get; private set; }

        public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            AddRange(items);
        } // end constructor PaginatedList

        /// <summary>
        /// Validate whether it has a previous page.
        /// </summary>
        public bool HasPreviousPage
        {
            get { return (PageIndex > 1); }
        } // end member field HasPreviousPage

        /// <summary>
        /// Validate whether it has next page.
        /// </summary>
        public bool HasNextPage
        {
            get { return (PageIndex < TotalPages); }
        } // end member field HasNextPage

        /// <summary>
        /// Create a paginated list of objects for the pagination, as an asychronous operation.
        /// </summary>
        /// <param name="source">data source</param>
        /// <param name="pageIndex">a specified page index</param>
        /// <param name="pageSize">the number of items in a page</param>
        /// <returns></returns>
        public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize)
        {
            var count = await source.CountAsync();
            var items = await source
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return new PaginatedList<T>(items, count, pageIndex, pageSize);
        } // end method CreateAsync
    } // end class PaginatedList
} // end namespace NewEraFlowerStore.Extensions