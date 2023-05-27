using System.ComponentModel.DataAnnotations;

namespace MyMail.Models
{
	public class ServerModel
	{
		[Display(Name = "Server Name: ")]
		public string ServerName { get; set; }

		[Display(Name = "SMTP Port: ")]
		public string SmtpPort { get; set; }

		[Display(Name = "POP3 Port: ")]
		public string Pop3Port { get; set; }

		[Display(Name = "IMAP Port: ")]
		public string ImapPort { get; set; }

	}
}
