using DinkToPdf;
using DinkToPdf.Contracts;
using HiSubmit.Application.Interfaces.PdfConverter;

namespace Web.Services;

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
        var pdfTemplateUrl = configuration.GetValue<string>("SiteURLConfiguration:BaseUrl") + "pdfTemplate";
        var ob = new ObjectSettings
        {
            PagesCount=true,
            HtmlContent=request.Content,
            HeaderSettings = new HeaderSettings()
            {
                
                //  HtmUrl = pdfTemplateUrl,
                Left = "HiSubmit.com",
                Right = DateTime.Now.ToShortDateString(),
                Spacing = 20,
                Center = "HiSubmit",
                Line = true
            },
            WebSettings={ DefaultEncoding="utf-8", PrintMediaType = true, LoadImages = true, Background = true }
        };

        var pdf = new HtmlToPdfDocument
        {
            Objects={ ob },
            GlobalSettings=setting
        };

        var s = converter.Convert(pdf);

        return Task.FromResult(s);
    }
}