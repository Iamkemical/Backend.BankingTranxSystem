using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.BankingTranxSystem.SharedServices.Helper;

public class PagedList<T> : List<T>
{
    public int CurrentPage { get; private set; }
    public int TotalPages { get; private set; }
    public int PageSize { get; private set; }
    public int TotalCount { get; private set; }
    public bool HasPrevious => (CurrentPage > 1);
    public bool HasNext => (CurrentPage < TotalPages);
    public string PaginationData { get; private set; }
    public string ContinuationToken { get; set; }
    public decimal Balance { get; private set; }
    public string Reference { get; private set; }

    public PagedList(List<T> items, string continuationToken)
    {
        ContinuationToken = continuationToken;

        AddRange(items);

        PaginationData = JsonConvert.SerializeObject(new
        {
            totalCount = TotalCount,
            pageSize = PageSize,
            currentPage = 0,
            totalPages = TotalPages,
            continuationToken
        });
    }

    public PagedList(List<T> items, int count, int pageNumber, int pageSize)
    {
        TotalCount = count;
        PageSize = pageSize;
        CurrentPage = pageNumber;
        TotalPages = (int)Math.Ceiling(count / (decimal)pageSize);
        AddRange(items);

        PaginationData = JsonConvert.SerializeObject(new
        {
            totalCount = TotalCount,
            pageSize = PageSize,
            currentPage = pageNumber,
            totalPages = TotalPages
        });
    }

    public PagedList(List<T> items,
                     int count,
                     int pageNumber,
                     int pageSize,
                     decimal balance,
                     string reference)
    {
        TotalCount = count;
        PageSize = pageSize;
        CurrentPage = pageNumber;
        TotalPages = (int)Math.Ceiling(count / (decimal)pageSize);
        Balance = balance;
        Reference = reference;
        AddRange(items);

        PaginationData = JsonConvert.SerializeObject(new
        {
            totalCount = TotalCount,
            pageSize = PageSize,
            currentPage = pageNumber,
            totalPages = TotalPages
        });
    }

    public static async Task<PagedList<T>> Create(IQueryable<T> source, int pageNumber, int pageSize)
    {
        var count = source.Count();
        var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
        return new PagedList<T>(items, count, pageNumber, pageSize);
    }
}

public class CachePagedList<T> where T : class
{
    public decimal Balance { get; set; }
    public string Reference { get; set; }
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public bool HasPrevious => (CurrentPage > 1);
    public bool HasNext => (CurrentPage < TotalPages);
    public IEnumerable<T> Items { get; set; }
}
