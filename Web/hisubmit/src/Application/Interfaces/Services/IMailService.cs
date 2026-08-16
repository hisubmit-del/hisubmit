using HiSubmit.Application.Requests.Mail;
using System.Threading.Tasks;

namespace HiSubmit.Application.Interfaces.Services
{
    public interface IMailService
    {
        Task SendAsync(MailRequest request);
    }
}