using System;

namespace Backend.BankingTranxSystem.SharedServices.Helper;

public class BaseResourceParameter
{
    const int maxPageSize = 100;
    private int _pageSize = 20;
    public int PageSize { get => _pageSize; set => _pageSize = (value > maxPageSize) ? maxPageSize : value; }
    public int PageNumber { get; set; } = 1;
}

public class BaseResourceWithDateParameter : BaseResourceParameter
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}

public class UserActivityResourceParameter
{
    const int maxPageSize = 30;
    private int _pageSize = 20;
    public int PageSize { get => _pageSize; set => _pageSize = (value > maxPageSize) ? maxPageSize : value; }
    /// <summary>
    /// The value is returned in the response header as "Continuation-Token". Use it for pagination.
    /// </summary>
    public string ContinuationToken { get; set; }
}
