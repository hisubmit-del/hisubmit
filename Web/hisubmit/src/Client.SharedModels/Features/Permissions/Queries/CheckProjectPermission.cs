namespace Hisubmit.Client.SharedModels.Features.Permissions.Queries;

public class CheckProjectPermissionQuery
{
    public int ProjectId { get; set; }
}


public enum ProjectPermissionResponse
{
    Read = 0,
    Write = 1
}