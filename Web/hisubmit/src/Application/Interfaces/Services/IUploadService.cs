using HiSubmit.Application.Requests;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Hisubmit.Client.SharedModels.Requests;

namespace HiSubmit.Application.Interfaces.Services
{
    public interface IUploadService
    {
        string UploadAsync(UploadRequest request);
        bool DeleteAsync(DeleteFileRequest request);
        bool ExistAsync(ExistFileRequest request);
        bool DeleteAsync(DeleteFileWithUploadTypeRequest request);
        Task AppendAsync(AppendFileRequest request);
    }
}