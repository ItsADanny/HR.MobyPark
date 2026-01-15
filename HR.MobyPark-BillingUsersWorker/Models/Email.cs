using System.Net.Mail;

public class Email
{

    public string To { get; set; }
    public readonly string from = "no-reply@mobypark.com";
    public string Subject { get; set; }
    public string Body { get; set; }
    
    public Email(string to, string subject, string body)
    {
        To = to;
        Subject = subject;
        Body = body;
    }

    public MailMessage toMailMessage()
    {
        MailMessage mail = new MailMessage();
        mail.From = new MailAddress(from);
        mail.To.Add(To);
        mail.Subject = Subject;
        mail.Body = Body;
        mail.IsBodyHtml = true;
        return mail;
    }

    public bool SendMail()
    {
        MailMessage mail = toMailMessage();
        SmtpClient SmtpServer = new SmtpClient("SMTP SERVER HERE");
        SmtpServer.Port = 587;
        SmtpServer.Credentials = new System.Net.NetworkCredential("EMAIL HERE", "PASSWORD HERE");
        SmtpServer.EnableSsl = true;
        try
        {
            SmtpServer.Send(mail);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to send email to {To}: {ex.Message}");
            return false;
        }
    }
}