using System.ComponentModel.DataAnnotations;

namespace MyMail.Models
{
    public class DownloadedMail
    {
        public int Id { get; set; } 
        public string SenderName { get; set; }
        public string SenderEmail { get; set; }
        public string RecipientName { get; set; }

        public string RecipientEmail { get; set; }

        public string Subject { get; set; }
        public string Body { get; set; }
    }
}
