using System.Threading.Tasks;

namespace HiSubmit.Application.Interfaces.GenerateQrCode;

public interface IGenerateQrCode
{
    Task<byte[]> Generate(string data);
}