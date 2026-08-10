using Hisubmit.Client.SharedModels.Enums;
using Hisubmit.Client.SharedModels.Wrapper;
using HiSubmit.Client.SharedModels.Extensions;
using System.ComponentModel.DataAnnotations;

namespace Hisubmit.Hisubmit.Client.SharedModels.Features.Payments
{
    public class PaymentFilterDto : PagedRequest
    {
        public string UserId { get; set; }
        public string SearchString { get; set; }
        public int? FestivalId { get; set; }
        public int? MasterFestivalId { get; set; }
        public string Code { get; set; }

        public CartItemType? ItemType { get; set; }
        public DateFilter CardDateFilter { get; set; } = new();

        public NumberFilter<decimal> IncomeFilter { get; set; } = new();
        public int? YearsRunning { get; set; }
    }

    public enum TimePeriod
    {
        [Display(Name = "Last Week")] Weekly = 0,
        [Display(Name = "last Month")] Monthly = 1,
        [Display(Name = "Last year")] Yearly = 2,
        [Display(Name = "Period")] Period = 3,
        [Display(Name = "=")] Equal = 4,
        [Display(Name = "Before")] Before = 5,
        [Display(Name = "After")] After = 6,

    }

    public enum NumberFilterType
    {
        [Display(Name = "=")] Equal = 0,
        [Display(Name = ">")] GreaterThan = 1,
        [Display(Name = "<")] LessThan = 2,
        [Display(Name = "range")] Range = 3
    }


    public class NumberFilter<T> where T : struct
    {
        public T? Number1 { get; set; }
        public T? Number2 { get; set; }

        public NumberFilterType NumberFilterType { get; set; }
    }

    public class NumberFilter : NumberFilter<int>;


    public class DateFilter
    {
        public TimePeriod? Period { get; set; }
        public DateTime? Date1 { get; set; }
        public DateTime? Date2 { get; set; }

        public DateTime? GetMinDateTime()
        {
            switch (Period)
            {
                case TimePeriod.Equal:
                    return Date1;
                case TimePeriod.Before:
                    return null;
                case TimePeriod.After:
                    return Date1;
                case TimePeriod.Period:
                    return Date1;
                case TimePeriod.Yearly:
                    return DateTime.Now.AddYears(-1);
                case TimePeriod.Monthly:
                    return DateTime.Now.AddMonths(-1);
                case TimePeriod.Weekly:
                    return DateTime.Now.AddDays(-7);
            }

            return null;
        }

        public DateTime? GetMaxDateTime()
        {
            switch (Period)
            {
                case TimePeriod.Equal:
                    return Date1;
                case TimePeriod.Before:
                    return Date1;
                case TimePeriod.After:
                    return null;
                case TimePeriod.Period:
                    return Date2;
                case TimePeriod.Yearly:
                    return DateTime.Now;
                case TimePeriod.Monthly:
                    return DateTime.Now;
                case TimePeriod.Weekly:
                    return DateTime.Now;
            }

            return null;
        }


        public DateOnly? GetMinDate()
        {
            if (GetMinDateTime()!=null)
                return DateOnly.FromDateTime(GetMinDateTime().Value);

            return null;
        }

        public DateOnly? GetMaxDate()
        {
            if (GetMaxDateTime()!=null)
                return DateOnly.FromDateTime(GetMaxDateTime().Value);

            return null;
        }


        public override string ToString()
        {
            if (GetMinDateTime() !=null && GetMaxDateTime() !=null)
                return $"From {GetMinDateTime()?.ToShortDateString()} To  {GetMaxDateTime()?.ToShortDateString()}";
            if (GetMinDateTime()!=null)
                return $"From  {GetMinDateTime()?.ToShortDateString()}";
            if (GetMaxDateTime()!=null)
                return $" To {GetMaxDateTime()?.ToShortDateString()}";
            return string.Empty;
        }
    }
}