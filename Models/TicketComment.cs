using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Titan_BugTracker.Models
{
    public class TicketComment
    {
        //Primary Key
        public int Id { get; set; }

        [DisplayName("Member Comment")]
        public string Comment { get; set; }

        [DisplayName("Date")]
        public DateTimeOffset Created { get; set; }

        //Ticket (Foreign Key)
        [DisplayName("Ticket")]
        public int TicketId { get; set; }

        //User (Foreign key)
        [DisplayName("Team Member")]
        public string UserId { get; set; }

        //Navigation Properties
        //Ticket
        public virtual Ticket Ticket { get; set; }

        //User
        public virtual BTUser User { get; set; }
    }
}