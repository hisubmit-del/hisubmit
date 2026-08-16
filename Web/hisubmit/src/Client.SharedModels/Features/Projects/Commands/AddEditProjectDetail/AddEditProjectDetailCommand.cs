using Hisubmit.Client.SharedModels.Features.Locatuions.Commands.AddEdit;
using Hisubmit.Client.SharedModels.Enums;
using Hisubmit.Client.SharedModels.Requests;

namespace Hisubmit.Client.SharedModels.Features.Projects.Commands.AddEditProjectDetail
{
    public class AddEditProjectDetailCommand 
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string WebSite { get; set; }
        public string Twitter { get; set; }
        public string Youtube { get; set; }
        public string Telegram { get; set; }
        public string WhatsApp { get; set; }
        public string Instagram { get; set; }
        public string SubTitle { get; set; }
        public string OriginalTitle { get; set; }
        public ProjectType ProjectType { get; set; }
        public bool HasNoneEnglishTitle { get; set; }
        public string EnglishBriefSynopsis { get; set; }
        public string OriginalBriefSynopsis { get; set; }

        //Submitter
        public bool UseCurrentUserInformation { get; set; }

        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public AddEditAddressCommand Address { get; set; }
        public DateTime? BirthDate { get; set; }
        public Gender Gender { get; set; }

        public string FileURl { get; set; }
        public UploadRequest FileUrlUploadRequest { get; set; }
        

        public string URL { get; set; }

        public UploadRequest UploadRequest { get; set; }

        //student project
        public bool StudentProject { get; set; }
        public string UniversityName { get; set; }
        public string StudentPhotoCard { get; set; }

        public AddEditProjectDetailCommand()
        {
            Address = new AddEditAddressCommand();
        }
    }

   
}