namespace Technova_CRM.Models.CustomModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public class Messages
    {
        public List<Template> TemplateContent { get; set; }       // List of templates
        public string TemplateName { get; set; }
        public string HtmlContent { get; set; }       // HTML content of the email
        public string TextContent { get; set; }       // Plain text content of the email
        public string Subject { get; set; }           // Subject of the email
        public string FromEmail { get; set; }         // Sender's email address
        public string FromName { get; set; }          // Sender's name
        public List<Recipient> To { get; set; }       // List of recipients
        public bool? Important { get; set; }          // Mark email as important
        public bool? PreserveRecipients { get; set; } // Preserve "To" recipients (do not show them as bcc)
        public List<Attachment> Attachments { get; set; }            // Attachments
        public List<Image> Images { get; set; }                 // Inline images
        public bool? Async { get; set; }              // Send asynchronously
    }

    public class Template
    {
        public string Name { get; set; }
        public string Content { get; set; }
    }

    public class Recipient
    {
        public string Email { get; set; }             // Recipient's email address
        public string Name { get; set; }              // Recipient's name
        public string Type { get; set; }              // Type of recipient ("to", "cc", "bcc")
    }

    public class Attachment
    {
        public string Type { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }             // base64-encoded string
    }

    public class Image
    {
        public string Type { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }             // base64-encoded string
    }
}
