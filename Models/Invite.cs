﻿using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Titan_BugTracker.Models
{
    public class Invite
    {
        public int Id { get; set; }

        [DataType(DataType.Date)]
        [DisplayName("Invite Date")]
        public DateTimeOffset InviteDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayName("Join Date")]
        public DateTimeOffset JoinDate { get; set; }

        [DisplayName("Code")]
        public Guid CompanyToken { get; set; }

        [EmailAddress]
        [Required]
        public string InviteeEmail { get; set; }

        public string Message { get; set; }

        [Required]
        [DisplayName("First Name")]
        public string FirstName { get; set; }

        [Required]
        [DisplayName("Last Name")]
        public string LastName { get; set; }

        public bool IsValid { get; set; }

        [DisplayName("Company")]
        public int CompanyId { get; set; }

        [DisplayName("Project")]
        public int ProjectId { get; set; }

        [DisplayName("Invitor")]
        public string InvitorId { get; set; }

        [DisplayName("Invitee")]
        public string InviteeId { get; set; }

        public virtual Company Company { get; set; }
        public virtual Project Project { get; set; }
        public virtual BTUser Invitor { get; set; }
        public virtual BTUser Invitee { get; set; }
    }
}