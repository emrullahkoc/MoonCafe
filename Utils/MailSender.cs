using System.Net.Mail;
using System.Net;
using System.Drawing;

namespace MoonCafe.Utils;

public static class MailSender
{
    public const string SENDERMAIL = "emrullahkocccc@outlook.com";
    public const string SENDERPASSWORD = "dw30C6f-03IzrOpBj{9wdCYq&!i6.z~VQ2PuBED{Z}PRkrp^wz";
    public static void Send(IEnumerable<string> mailAddresses, string title,
        string message)
    {
        SmtpClient client = new SmtpClient();
        client.Port = 587;
        client.Host = "smtp-mail.outlook.com";
        client.EnableSsl = true;
        client.Timeout = 50000;

        string senderMail = SENDERMAIL;
        string senderPassword = SENDERPASSWORD;
        client.Credentials = new NetworkCredential(senderMail, senderPassword);

        MailMessage mail = new MailMessage();

        mail.From = new MailAddress(senderMail, "Moon Cafe");

        foreach (string mailAddress in mailAddresses)
        {
            mail.To.Add(mailAddress);
        }

        mail.Subject = title;
        mail.IsBodyHtml = true;
        mail.Body = message;

        client.Send(mail);
    }

    public static void SendAdmin(string title, string message, Stream imageStream, string imageName) //IMAGE ADD
    {
        SmtpClient client = new SmtpClient();
        client.Port = 587;
        client.Host = "smtp-mail.outlook.com";
        client.EnableSsl = true;
        client.Timeout = 50000;

        string senderMail = SENDERMAIL;
        string senderPassword = SENDERPASSWORD;
        client.Credentials = new NetworkCredential(senderMail, senderPassword);

        MailMessage mail = new MailMessage();

        mail.From = new MailAddress(senderMail, "Moon Cafe");
        mail.To.Add("emrullahkoc.1995@gmail.com");

        mail.Subject = title;
        mail.IsBodyHtml = true;
        mail.Body = message;

        if (imageStream != null && !string.IsNullOrEmpty(imageName))
        {
            Attachment attachment = new Attachment(imageStream, imageName);
            mail.Attachments.Add(attachment);
        }

        client.Send(mail);
    }
}
