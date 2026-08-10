using DinkToPdf;
using DinkToPdf.Contracts;
using HiSubmit.Application.Interfaces.PdfConverter;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace HiSubmit.Server.Services;

public class PdfGenerator(IConverter converter, IConfiguration configuration) : IPdfGenerator
{
    public Task<byte[]> GenerateFile(PdfGeneratorRequest request)
    {
        var marginSetting = new MarginSettings
        {
            Top = 20,
            Left = 20,
            Right = 20,
            Bottom = 20,
        };
        var setting = new GlobalSettings
        {
            Margins=marginSetting,
            PaperSize=PaperKind.A4,
            ColorMode=ColorMode.Color,
            DocumentTitle=request.DocTitle,
            Orientation=Orientation.Portrait,
            
        };
        var pdfTemplateUrl= configuration.GetValue<string>("SiteURLConfiguration") + "pdfTemplate";
        var ob = new ObjectSettings
        {
            PagesCount=true,
            HtmlContent=request.Content,
            HeaderSettings = new HeaderSettings()
            {
                HtmUrl = pdfTemplateUrl,
                Spacing = 20
            },
            WebSettings={DefaultEncoding="utf-8",PrintMediaType = true,LoadImages = true,Background = true}
        };

        var pdf = new HtmlToPdfDocument
        {
            Objects={ ob },
            GlobalSettings=setting
        };

        var s= converter.Convert(pdf);

        return Task.FromResult(s);
    }
}