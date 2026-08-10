using System.Collections.Generic;
using System.Linq;

namespace HiSubmit.Client.SharedModels.Extensions;

public static class ListExtensions
{
    public static List<int> DeleteZeroId(this List<int> ids)
    {
        var zeroIdExist= ids.Any(id=>id==0);
        if (zeroIdExist)
        {
            ids.Remove(0);
        }

        return ids;
    }
}