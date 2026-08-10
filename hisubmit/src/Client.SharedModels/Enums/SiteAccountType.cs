namespace Hisubmit.Client.SharedModels.Enums;

public enum SiteAccountType:short
{
    User=0,
    Admin=2,
    Festival=1,
}


public enum NotificationType:short
{
    AdminNewRegister=0,
    AdminNewFestival=1,
    AdminReleaseFestivalRequest=2,
    AdminSpecialFestivalRequest=3,
    AdminReportViolationFestival=4,
    AdminAdvertiseRequest=5,
    AdminReceivedMessage=6,
    FestivalNewSubmit=7,
    FestivalSoldTicket=8,
    FestivalSoldProduct=9,
    FestivalAnsweredReleasedRequest=10,
    FestivalAnsweredSpecificRequest=11,
    FestivalRefereeAdded=12,
    FestivalReceivedMessage=13,
    UserChangeSubmitStatus=14,
    UserChangedRefereeStatus=15,
    UserReceivedMessage=16,
    FestivalRefereeSubmitJudgingResult=17,
    RefereeAddToProject=18,
    AdminNewAddedTicketOrBadge=19,
    AdminNewAddedProduct = 20
}