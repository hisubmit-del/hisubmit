using System;
using System.Linq.Expressions;

namespace HiSubmit.Application.Interfaces.Services.BackGroundJob;

public interface IBackGroundJobService
{
    string AddEnqueue(Expression<Action> methodCall);

    string AddEnqueue<T>(Expression<Action<T>> methodCall);

    string AddContinuations(Expression<Action> methodCall, string jobId);

    string AddContinuations<T>(Expression<Action<T>> methodCall, string jobId);
    string AddSchedule(Expression<Action> methodeCall, DateTime time);

    string AddSchedule(Expression<Action> methodCall, RecurringTime recurringTime, double time);

    string AddSchedule<T>(Expression<Action<T>> methodCall, RecurringTime recurringTime, double time);
    void AddRecurring(Expression<Action> methodCall, CornJob cron, int step, string name);
    void Delete(string jobId);
    void Delete(params string[] jobsId);
}

public enum RecurringType
{
    Daily,
    Minutely,
    Hourly,
    Weekly,
    Monthly,
    Yearly
}

public enum RecurringTime
{
    Milliseconds,
    Seconds,
    Minutes,
    Hours,
    Day
}

public enum CornJob
{
    Never,
    Daily,
    Weekly,
    Yearly,
    Hourly,
    Monthly,
    Minutely
}