using HiSubmit.Application.Features.AdminFestival.Queries.GetAllFestival;
using HiSubmit.Application.Specifications.Base;
using HiSubmit.Domain.Entities.Festivals;
using System;
using System.Linq;
using Hisubmit.Client.SharedModels.Enums;
using Hisubmit.Client.SharedModels.Features.AdminFestival.Queries.GetAllFestival;
using FestivalStatus = HiSubmit.Domain.Enums.FestivalStatus;
using FestivalFeeStatus = HiSubmit.Domain.Enums.FeeStatus;

namespace HiSubmit.Application.Specifications.Festivals;

public class FestivalFilterSpecification : HeroSpecification<Festival>
{
    public FestivalFilterSpecification(GetAllFestivalQuery query)
    {
        Includes.Add((p)=>p.DeadLines);
        Includes.Add(p=>p.FestivalFestivalFoci);
        Includes.Add(p=>p.FestivalArtCategories);
        Criteria = festival =>
                (string.IsNullOrWhiteSpace(query.SearchString) || festival.Name.Contains(query.SearchString) ||
                 festival.Description.Contains(query.SearchString))
                && (!query.OpenOnly || (festival.OpeningDate <= DateTime.Now && festival.NotificationDate >= DateTime.Now))
                && (query.IsActive == null || festival.IsActive == query.IsActive)
                && (query.FeeStatus == null || festival.FeeStatus == (FestivalFeeStatus)query.FeeStatus)
                && (query.OpeningDateFrom == null || query.OpeningDateFrom < festival.OpeningDate)
                && (query.OpeningDateTo == null || query.OpeningDateTo > festival.OpeningDate)
                && (string.IsNullOrWhiteSpace(query.Name) || festival.Name.Contains(query.Name))
                
                && (query.FilmFestival == null || festival.FilmFestival == query.FilmFestival)
                && (query.ScreenWritingWriter == null || festival.ScreenWritingWriter == query.ScreenWritingWriter)
                && (query.PhotographicContest == null || festival.PhotographicContest == query.PhotographicContest)
                && (query.MusicContest == null || festival.MusicContest == query.MusicContest)
                && (query.OnlineFestival == null || festival.OnlineFestival == query.OnlineFestival)
                && (query.FestivalStatus == null || festival.FestivalStatus ==(FestivalStatus) query.FestivalStatus)
                &&(query.FestivalType ==null || 
                   (query.FestivalType==FestivalType.FilmFestival && festival.FilmFestival)||
                   (query.FestivalType==FestivalType.MusicContest && festival.MusicContest)||
                   (query.FestivalType==FestivalType.ScreenWritingWriter && festival.ScreenWritingWriter)||
                   (query.FestivalType==FestivalType.PhotographicContest && festival.PhotographicContest)||
                   (query.FestivalType==FestivalType.OnlineFestival && festival.OnlineFestival)
                   )
                && (query.YearsRunningRangeType == RangeType.NotSelected ||
                    (query.YearsRunningRangeType == RangeType.Equal && query.YearsRunningFirst == festival.YearsRunning) ||
                    (query.YearsRunningRangeType == RangeType.After && query.YearsRunningFirst <= festival.YearsRunning) ||
                    (query.YearsRunningRangeType == RangeType.Before && query.YearsRunningFirst >= festival.YearsRunning) ||
                    (query.YearsRunningRangeType == RangeType.Between && query.YearsRunningFirst <= festival.YearsRunning &&
                     query.YearsRunningSecond >= festival.YearsRunning)
                )
                && (query.Runtime == 0 || (festival.MinimomLenght != null || (festival.MinimomLenght <= query.Runtime) ||
                                           (festival.MaximomLenght != null || (festival.MaximomLenght >= query.Runtime)))
                )
                && (query.CountryId == 0 || festival.Address.CountryId == query.CountryId)
                && (!query.Categories.Any() || festival.FestivalArtCategories
                    .Any(l => query.Categories.Contains(l.ArtCategoryId)))
                && (!query.Focuses.Any() || festival.FestivalFestivalFoci
                    .Any(l => query.Focuses.Contains(l.FestivalFocusId)))
                && (query.FeeRangeType == RangeType.NotSelected ||
                    (query.FeeRangeType == RangeType.Equal) ||
                    (query.FeeRangeType == RangeType.After && query.FeeFirst <= festival.MaxFee) ||
                    (query.FeeRangeType == RangeType.Before && query.FeeFirst >= festival.MinFee) ||
                    (query.FeeRangeType == RangeType.Between && (query.FeeFirst <= festival.MaxFee) &&
                     query.FeeSecond >= festival.MinFee)
                )
                
               // &&(command.EntryDeadlineFrom==null || p.DeadLines.Any(deadLine=>deadLine.Date>=command.EntryDeadlineFrom))
                &&(query.EntryDeadlineTo==null || festival.DeadLines.Any(deadLine=>deadLine.Date<=query.EntryDeadlineTo))
                &&(query.EventDateTo==null || festival.EventStartDate<=query.EventDateTo)
                &&(query.TicketOnly==false || (festival.Venues !=null && festival.Venues.SelectMany(l=>l.Tickets).Any()) )
                &&(query.Category==null || festival.FestivalArtCategories.Any(l=>l.ArtCategoryId==query.Category))
                &&(query.Focus==null || festival.FestivalFestivalFoci.Any(l=>l.FestivalFocusId==query.Focus))
                &&( !query.PublicOnly || festival.Public)
                
                &&(query.FeeMinVal==null || query.FeeMinVal==0 || festival.MinFee>=query.FeeMinVal)
                
                &&(query.FeeMaxVal ==null || query.FeeMaxVal== 0 || festival.MinFee<=query.FeeMaxVal)
                
                &&(query.YearsRunningMinVal==null ||query.YearsRunningMinVal==0  || festival.YearsRunning>=query.YearsRunningMinVal.Value)
                &&(query.YearsRunningMaxVal==null ||query.YearsRunningMaxVal==0  || festival.YearsRunning<=query.YearsRunningMaxVal.Value)
                &&(query.IsActivePeriod==null || query.IsActivePeriod==festival.IsActivePeriod)
                // && (
                //     command.EntryDeadlineRangeType == RangeType.NotSelected ||
                //     command.EntryDeadlineRangeType == RangeType.Equal ||
                //         (command.EntryDeadlineRangeType == RangeType.After 
                //          && p.DeadLines.Any(deadline => deadline.Date >= command.EntryDeadlineFrom)) ||
                //         (command.EntryDeadlineRangeType == RangeType.Before 
                //          && p.DeadLines.Any(deadline => deadline.Date <= command.EntryDeadlineFrom)) ||
                //         (command.EntryDeadlineRangeType == RangeType.Between 
                //          && p.DeadLines.Any(deadline =>
                //          deadline.Date >= command.EntryDeadlineFrom && deadline.Date <= command.EntryDeadlineTo))
                //     )
            ;
    }
}
