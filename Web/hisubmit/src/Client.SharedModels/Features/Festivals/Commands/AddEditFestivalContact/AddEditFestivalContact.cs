using Hisubmit.Client.SharedModels.Features.Locatuions.Commands.AddEdit;
using Hisubmit.Client.SharedModels.Enums;

namespace Hisubmit.Client.SharedModels.Features.Festivals.Commands.AddEditFestivalContact
{
    public class AddEditFestivalContactCommand 
    {
        public int Id { get; set; }
        //Contact
        public string WebSite { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public AddEditAddressCommand Address { get; set; }

        //Social Media
        public string Facebook { get; set; }
        public string Twitter { get; set; }
        public string Instagram { get; set; }
        public string WhatsAppNumber { get; set; }
        public string Telegram { get; set; }
        public string Youtube { get; set; }

        //Submission Address
        public bool SeparateSubmissiionAddress { get; set; }
        public AddEditAddressCommand SubmissionAddress { get; set; }

        public bool OnlineEvent { get; set; }

        public bool ChangesNotAllowed { get; set; }
        public  FestivalStatus FestivalStatus { get; set; }

        public AddEditFestivalContactCommand()
        {
            Address = new AddEditAddressCommand();
            SubmissionAddress = new AddEditAddressCommand();
        }
    }
}
