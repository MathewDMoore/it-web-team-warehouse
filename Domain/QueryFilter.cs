using System;
using Common;

namespace Domain
{
    public class QueryFilter
    {
        public QueryFilter(SortDirection orderBy)
        {
            _orderByDirection = orderBy;
            Page = 1;
        }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string OrderBy { get; set; }
        public int Page { get; set; }
        public int ItemsPerPage { get; set; }
        private readonly SortDirection _orderByDirection;
        public int TotalResults { get; set; }
        public string OrderByDirection
        {
            get { return _orderByDirection.Value(); }
        }

        public int Skip
        {
            get
            {
                var skip = 0;
                if (Page > 1)
                {
                    skip = (Page - 1) * ItemsPerPage;
                }
                return skip;
            }
        }

        public int NumberOfPages
        {
            get { return (int)Math.Ceiling((TotalResults / (decimal)ItemsPerPage)); }
        }

        public enum SortDirection
        {
            [EnumString("ASC")]
            Ascending,

            [EnumString("DESC")]
            Decending
        }
    }
}