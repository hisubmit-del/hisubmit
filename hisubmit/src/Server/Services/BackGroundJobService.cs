using System;
using System.Linq.Expressions;
using Hangfire;
using HiSubmit.Application.Interfaces.Services.BackGroundJob;

namespace HiSubmit.Server.Services;

public class BackGroundJobService : IBackGroundJobService
{
    private readonly IBackgroundJobClient _backgroundClient;
    private readonly IRecurringJobManager _recurringJobManager;

    public BackGroundJobService(IBackgroundJobClient backgroundJobClient, IRecurringJobManager recurringJobManager)
    {
        _backgroundClient = backgroundJobClient;
        _recurringJobManager = recurringJobManager;
    }

    public string AddEnqueue(Expression<Action> methodCall)
    {
        return _backgroundClient.Enqueue(methodCall);
    }

    public string AddEnqueue<T>(Expression<Action<T>> methodCall)
    {
        return _backgroundClient.Enqueue<T>(methodCall);
    }

    public string AddContinuations(Expression<Action> methodCall, string jobId)
    {
        return _backgroundClient.ContinueJobWith(jobId, methodCall);
    }

    public string AddContinuations<T>(Expression<Action<T>> methodCall, string jobId)
    {
        return _backgroundClient.ContinueJobWith<T>(jobId, methodCall);
    }

    public string AddSchedule(Expression<Action> methodeCall, DateTime time)
    {
        return _backgroundClient.Schedule(methodeCall, time);
    }

    public string AddSchedule(Expression<Action> methodCall, RecurringTime recurringTime, double time)
    {
        switch (recurringTime)
        {
            case RecurringTime.Milliseconds:
                return _backgroundClient.Schedule(methodCall, TimeSpan.FromMilliseconds(time));

            case RecurringTime.Seconds:
                return _backgroundClient.Schedule(methodCall, TimeSpan.FromSeconds(time));

            case RecurringTime.Minutes:
                return _backgroundClient.Schedule(methodCall, TimeSpan.FromMinutes(time));

            case RecurringTime.Hours:
                return _backgroundClient.Schedule(methodCall, TimeSpan.FromHours(time));

            case RecurringTime.Day:
                return _backgroundClient.Schedule(methodCall, TimeSpan.FromDays(time));

            default:
                return _backgroundClient.Schedule(methodCall, TimeSpan.FromMinutes(time));
        }
    }

    public string AddSchedule<T>(Expression<Action<T>> methodCall, RecurringTime recurringTime, double time)
    {
        switch (recurringTime)
        {
            case RecurringTime.Milliseconds:
                return _backgroundClient.Schedule<T>(methodCall, TimeSpan.FromMilliseconds(time));

            case RecurringTime.Seconds:
                return _backgroundClient.Schedule<T>(methodCall, TimeSpan.FromSeconds(time));

            case RecurringTime.Minutes:
                return _backgroundClient.Schedule<T>(methodCall, TimeSpan.FromMinutes(time));

            case RecurringTime.Hours:
                return _backgroundClient.Schedule<T>(methodCall, TimeSpan.FromHours(time));

            case RecurringTime.Day:
                return _backgroundClient.Schedule<T>(methodCall, TimeSpan.FromDays(time));

            default:
                return _backgroundClient.Schedule<T>(methodCall, TimeSpan.FromMinutes(time));
        }
    }

    public void AddRecurring(Expression<Action> methodCall, CornJob cron, int step, string name)
    {
        switch (cron)
        {
            case CornJob.Never:
                break;
            case CornJob.Daily:
                _recurringJobManager.AddOrUpdate(name, methodCall, Cron.Daily());
                break;
            case CornJob.Weekly:
                _recurringJobManager.AddOrUpdate(name, methodCall, Cron.Weekly());
                break;
            case CornJob.Yearly:
                _recurringJobManager.AddOrUpdate(name, methodCall, Cron.Yearly());
                break;
            case CornJob.Hourly:
                _recurringJobManager.AddOrUpdate(name, methodCall, Cron.Hourly());
                break;
            case CornJob.Monthly:
                _recurringJobManager.AddOrUpdate(name, methodCall, Cron.Monthly());
                break;
            case CornJob.Minutely:
                _recurringJobManager.AddOrUpdate(name, methodCall, Cron.Minutely());
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(cron), cron, null);
        }
    }

    public void Delete(string jobId)
    {
        if (!string.IsNullOrWhiteSpace(jobId))
        {
            _backgroundClient.Delete(jobId);
        }
    }

    public void Delete(params string[] jobsId)
    {
        foreach (var jobId in jobsId)
        {
            Delete(jobId);
        }
    }
}