﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Titan_BugTracker.Models
{
    public class Project
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        [DisplayName("Name")]
        public string Name { get; set; }

        [Required]
        [DisplayName("Description")]
        public string Description { get; set; }

        [DataType(DataType.Date)]
        [DisplayName("Start Date")]
        public DateTimeOffset StartDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayName("End Date")]
        public DateTimeOffset EndDate { get; set; }

        [NotMapped]
        [DataType(DataType.Upload)]
        public IFormFile FormFile { get; set; }

        [DisplayName("File Name")]
        public string FileName { get; set; }

        public byte[] FileData { get; set; }

        [DisplayName("File Extension")]
        public string FileContentType { get; set; }

        [DisplayName("Archived")]
        public bool Archived { get; set; }

        public int? CompanyId { get; set; }
        public int? ProjectPriorityId { get; set; }

        public virtual Company Company { get; set; }
        public virtual ProjectPriority ProjectPriority { get; set; }

        public virtual ICollection<BTUser> Members { get; set; } = new HashSet<BTUser>();
        public virtual ICollection<Ticket> Tickets { get; set; } = new HashSet<Ticket>();
    }
}