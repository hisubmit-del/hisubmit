using System.Collections.Generic;

namespace HiSubmit.Application.Requests.Mail
{
    public class MailRequest
    {
        public string To { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string From { get; set; }
        public  List<MailAttachment> Attachments { get; set; }

        public MailRequest()
        {
            Attachments = new List<MailAttachment>();
        }
    }

    public class MailAttachment
    {
        public  string Name { get; set; }
        public  byte[] File { get; set; }
        public  string ContentType { get; set; }
    }
}