﻿using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Titan_BugTracker.Models
{
    public class Notification
    {
        public int Id { get; set; }

        [Required]
        [DisplayName("Title")]
        public string Title { get; set; }

        [Required]
        [DisplayName("Message")]
        public string Message { get; set; }

        [DisplayName("Date Modified")]
        public DateTimeOffset Created { get; set; }

        [DisplayName("Has been viewed")]
        public bool Viewed { get; set; }

        [DisplayName("Ticket")]
        public int TicketId { get; set; }

        [Required]
        [DisplayName("Recipient")]
        public string RecipientId { get; set; }

        [Required]
        [DisplayName("Sender")]
        public string SenderId { get; set; }

        public virtual Ticket Ticket { get; set; }
        public virtual BTUser Recipient { get; set; }
        public virtual BTUser Sender { get; set; }
    }
}