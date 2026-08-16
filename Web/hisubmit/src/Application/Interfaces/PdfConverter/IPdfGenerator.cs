using System.Threading.Tasks;

namespace HiSubmit.Application.Interfaces.PdfConverter;

public interface IPdfGenerator
{
    Task<byte[]> GenerateFile(PdfGeneratorRequest request);
}

public class PdfGeneratorRequest
{
    public  string Content { get; set; }
    public  string DocTitle { get; set; }
}
