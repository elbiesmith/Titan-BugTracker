using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Titan_BugTracker.Models
{
    public class TicketHistory
    {
        //Properties
        public int Id { get; set; }

        [DisplayName("Updated Item")]
        public string Property { get; set; }

        [DisplayName("Previous")]
        public string OldValue { get; set; }

        [DisplayName("Current")]
        public string NewValue { get; set; }

        [DisplayName("Date Modified")]
        public DateTimeOffset Created { get; set; }

        [DisplayName("Description of Change")]
        public string Description { get; set; }

        //Foreign Keys
        [DisplayName("Team Member")]
        public string UserId { get; set; }

        [DisplayName("Ticket")]
        public int TicketId { get; set; }

        //Nav Properties
        public virtual BTUser User { get; set; }

        public virtual Ticket Ticket { get; set; }
    }
}