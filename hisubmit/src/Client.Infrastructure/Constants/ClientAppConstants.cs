using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudBlazor;

namespace HiSubmit.Client.Infrastructure.Constants
{
    public static class ClientAppConstants
    {
        public static readonly string NoImageUrl = "/media/image.png";
        public static readonly string AudioCover = "/media/32.jpg";
        public static readonly string[] ChartPalette =
        {
            "#2DB482","#5db9c7" ,"#183B56",Colors.Teal.Lighten1, Colors.Orange.Darken2, Colors.Pink.Darken2,
            Colors.Red.Darken2, Colors.Purple.Darken1, Colors.DeepPurple.Darken1
        };

        public static readonly ChartOptions ChartOptions = new ChartOptions()
        {
            ChartPalette = ChartPalette,
            XAxisLines = true,
            YAxisLines = true,
        };
        
        public  static  readonly  string LogoUrl ="/media/logo/logohixl.png";
        public static readonly string SmallLogoUrl = "/media/logo/logohisubm.png";
        public static readonly string TicketImg = "/img/Ticket.jpeg";

        public static string[] Colorpallets =
        {

        };
    }
}