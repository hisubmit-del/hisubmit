using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using Hisubmit.Client.SharedModels.Enums;

namespace HiSubmit.Client.SharedModels.Extensions;

public static class EnumExtensions
{
    public static string ToDisplay(this Enum value, DisplayProperty displayProperty = DisplayProperty.Name)
    {

        var fieldInfo = value.GetType().GetField(value.ToString());
        var attributes = fieldInfo.GetCustomAttributes(false);

        foreach (var attribute in attributes)
        {
            if (attribute is DisplayAttribute displayAttribute)
            {
                return displayAttribute.Name;
            }
        }

        return value.ToString();

        //if (value == null || value.GetType().GetField(value.ToString())==null)
        //    return string.Empty;
        
        //var attribute = value.GetType().GetField(value.ToString())!
        //    .GetCustomAttributes<DisplayAttribute>(false)
        //    .FirstOrDefault();
        

        //var propValue = attribute?.GetType().GetProperty(displayProperty.ToString())
        //    ?.GetValue(attribute, null);
        //return propValue != null ? propValue.ToString() : value.ToString();
    }
    
        public static string ToDescriptionString(this Enum val)
        {
            var attributes = (DescriptionAttribute[])val.GetType().GetField(val.ToString())
                .GetCustomAttributes(typeof(DescriptionAttribute), false);

            return attributes.Length > 0
                ? attributes[0].Description
                : val.ToString();
        }



        public static string GetAcceptedFormat(this FileFormat format)
        {
            switch (format)
            {
                case FileFormat.Doc:
                    return ".doc,.docx,.txt,.rtf,application/msword,application/vnd.openxmlformats-officedocument.wordprocessingml.document,text/plain,application/rtf";
                case FileFormat.PDF:
                    return "application/pdf";
                case FileFormat.Image:
                    return "image/png, image/jpeg";

                //case FileFormat.xslx:
                //    return
                //        ".csv, application/vnd.openxmlformats-officedocument.spreadsheetml.sheet, application/vnd.ms-excel";
                case FileFormat.ZipRar:
                    return
                        ".zip,.rar,.7z,.gz";

                case FileFormat.Video:
                    return
                        ".mkv, video/mp4,video/x-m4v,video/*";
                case FileFormat.Music:
                    return "audio/mp3,audio/*;capture=microphone";
            }

            return string.Empty;
    }
}





public  enum  DisplayProperty {
    Description,
    GroupName,
    Name,
    Prompt,
    ShortName,
    Order
}
