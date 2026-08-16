using HiSubmit.Application.Enums;
using Hisubmit.Client.SharedModels.Enums;

namespace HiSubmit.Application.Requests
{
    public class DeleteFileRequest
    {
        public string RelativeDirectory { get; set; }
       
    }

    public class DeleteFileWithUploadTypeRequest
    {
        public string Name { get; set; }
        public UploadType  UploadType { get; set; }
    }
}