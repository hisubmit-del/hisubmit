namespace Hisubmit.Client.SharedModels.Enums;

public enum UserSpecialAccountStatus
{
    DontPaid,
    Expired,
    Open,
    Cancel
}

public enum DiscountValueType : byte
{
    Percentage = 0,
    Amount = 1
}