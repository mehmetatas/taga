namespace Taga.Core.Mail
{
    public interface IMailSender
    {
        void Send(MailMessage mailMessage);
    }
}