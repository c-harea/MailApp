﻿using System.ComponentModel.DataAnnotations;

namespace MyMail.Models
{
    public class SendMailModel
    {
        [Display(Name = "My Name")]
        public string MyName { get; set; }

        [Display(Name = "Recipient Name")]
        public string RecipientName { get; set; }

        [Display(Name = "Recipient Email")]
        public string RecipientEmail { get; set; }

        [Display(Name = "Subject")]
        public string Subject { get; set; }

        public string Body { get; set; }
        public IFormFile Attachment { get; set; }
    }
}