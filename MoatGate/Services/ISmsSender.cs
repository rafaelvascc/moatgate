using System.Threading.Tasks;

namespace MoatGate.Services
{
    public interface ISmsSender
    {
        Task SendSMSAsync(string number, string message);
        Task SendPhoneNumberChangeConfirmationSMSAsync(string number, string callbackUrl);
    }
}
