using MudBlazor;
using System;
using System.Collections;
using System.Collections.Generic;

namespace AdminDashboard.Wasm.Models
{
    public class ChatUser
    {
        public string UserName { get; set; }
        public string UserRoleColor { get; set; }
        public Color OnlineStatus { get; set; }
        public bool Spotify { get; set; }
        public string AvatarUrl { get; set; }
        public Color AvatarColor { get; set; }
    }

    public class CheckBoxItem<T> 
    {
        public T Value { get; set; }
        public string Name { get; set; }
        public bool IsSelected { get; set; }

        public static ICollection<CheckBoxItem<T>> CovertToCheckboxItems<TItem>
            (ICollection<TItem> objects,string value="Id",string name="Name" )
        {
            var t = typeof(TItem);
            var items = new List<CheckBoxItem<T>>();
            foreach (var obj in objects)
            {
                var item = new CheckBoxItem<T>
                {
                    Name = t.GetProperty(name)?.GetValue(obj)!.ToString(),
                    Value = (T) t.GetProperty(value)?.GetValue(obj)
                };
                items.Add(item);
            }

            return items;
        }
    }

    public class CheckBoxItem : CheckBoxItem<int>
    {
        
    }
}