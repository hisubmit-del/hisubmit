using System;

namespace Hisubmit.Client.SharedModels.CustomeAttribute;

[AttributeUsage(AttributeTargets.Class)]
public class CostumePermissionAttribute:Attribute
{
    public  PermissionType PermissionType { get; set; }

    public CostumePermissionAttribute()
    {
            
    }

    public CostumePermissionAttribute(PermissionType permissionType)
    {
        PermissionType = permissionType;
    }
}

public enum PermissionType
{
    Admin,
    Festival,
    Referee,
    Artist
}