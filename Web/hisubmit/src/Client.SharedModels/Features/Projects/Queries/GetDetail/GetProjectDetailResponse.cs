using Hisubmit.Client.SharedModels.Features.Locatuions.Commands.AddEdit;
using Hisubmit.Client.SharedModels.Enums;
using System;

namespace Hisubmit.Client.SharedModels.Features.Projects.Queries.GetDetail
{
    public class GetProjectDetailResponse
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public  string SubTitle { get; set; }
        public ProjectType ProjectType { get; set; }
        public string EnglishBriefSynopsis { get; set; }
        public bool HasNoneEnglishTitle { get; set; }
        public string OriginalTitle { get; set; }
        public string OriginalBriefSynopsis { get; set; }
        public string WebSite { get; set; }
        public string Twitter { get; set; }
        public string Youtube { get; set; }
        public string Instagram { get; set; }
        public string Telegram { get; set; }
        public string WhatsApp { get; set; }

        public string UserId { get; set; }

        //Submitter information
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public AddEditAddressCommand Address { get; set; }
        public DateTime BirthDate { get; set; }
        public Gender Gender { get; set; }

        public string UserImageUrl { get; set; }
        public string UserFullName { get; set; }
        
        //project File
        public bool IsLocalFile { get; set; }
        public string FileURl { get; set; }
        public string LocalFileURL { get; set; }
        public string Password { get; set; }


        public string URL { get; set; }

        
        
        //student project
        public bool StudentProject { get; set; }
        public  string UniversityName { get; set; }
        public  string StudentPhotoCard { get; set; }
        
        
        public  DateTime CreateOn { get; set; }
    }
}
