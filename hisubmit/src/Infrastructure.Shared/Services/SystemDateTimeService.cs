using HiSubmit.Application.Interfaces.Services;
using System;

namespace HiSubmit.Infrastructure.Shared.Services
{
    public class SystemDateTimeService : IDateTimeService
    {
        public DateTime NowUtc => DateTime.UtcNow;
    }
}