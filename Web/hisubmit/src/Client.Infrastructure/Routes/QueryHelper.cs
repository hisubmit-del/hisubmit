using MudBlazor.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSubmit.Client.Infrastructure.Routes;

public static class QueryHelper
{
    public static string GetQueryString<Model>(Model request)
    {
        // Get all properties on the object
        var properties = request.GetType().GetProperties()
            .Where(x => x.CanRead)
            .Where(x => x.GetValue(request, null) != null)
            .ToDictionary(x => x.Name, x => x.GetValue(request, null));

        // Get names for all IEnumerable properties (excl. string)
        var propertyNames = properties
            .Where(x => !(x.Value is string) && x.Value is IEnumerable)
            .Select(x => x.Key)
            .ToList();

        // Concat all IEnumerable properties into a comma separated string
        foreach (var key in propertyNames)
        {
            var valueType = properties[key].GetType();
            var valueElemType = valueType.IsGenericType
                ? valueType.GetGenericArguments()[0]
                : valueType.GetElementType();
            if (valueElemType.IsPrimitive || valueElemType == typeof (string))
            {
                var enumerable = properties[key] as IEnumerable;
                properties[key] = string.Join(',', enumerable.Cast<object>());
            }
        }

        // Concat all key/value pairs into a string separated by ampersand
        return string.Join("&", properties
            .Select(x => string.Concat(
                Uri.EscapeDataString(x.Key), "=",
                Uri.EscapeDataString(x.Value.ToString()))));
    }
        
        //
        // var properties = model.GetType().GetProperties();
        // var items = new List<string>();
        //
        // foreach (var item in properties)
        // {
        //     if (item != null)
        //     {
        //         if (!(item.GetValue(model) is IEnumerable))
        //         {
        //             items.Add(item.Name + "=" + item.GetValue(model));
        //         }
        //         else
        //         {
        //             var listItem = new List<string>();
        //
        //             foreach (var i in item.GetValue(model) as IEnumerable)
        //             {
        //                 listItem.Add(i.ToString());
        //             }
        //
        //             items.Add(item.Name + "=" + string.Join(",", listItem));
        //         }
        //     }
        // }
        //
        // return string.Join("&", items);
   // }
}

public class BaseEndPoint
{
    private readonly string _route;

    public string Route { get; }

    public BaseEndPoint(string route)
    {
        _route = route;
    }

    public string GenerateUrl<TModel>(string action, TModel query)
    {
        var route = $"{_route}/{action}?{QueryHelper.GetQueryString(query)}";
        return route;
    }

    public string GenerateUrl(string action)
    {
        var route = $"{_route}/{action}";
        return route;
    }

    public string GenerateUrl<TModel>(TModel model)
    {
        return $"{_route}?{QueryHelper.GetQueryString(model)}";
    }
}