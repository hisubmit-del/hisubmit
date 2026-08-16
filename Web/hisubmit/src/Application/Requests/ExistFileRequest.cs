using HiSubmit.Application.Enums;
using Hisubmit.Client.SharedModels.Enums;
using Microsoft.AspNetCore.Http;

namespace HiSubmit.Application.Requests
{
    public class ExistFileRequest
    {
        public string  Name { get; set; }
        public UploadType UploadType { get; set; }

    }

    public class AppendFileRequest
    {
        public IFormFile  File { get; set; }
        public UploadType UploadType { get; set; }
    }
}