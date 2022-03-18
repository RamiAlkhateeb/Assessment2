using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assessment.Common.Models.Database
{
    public class MailLog
    {
        public int Id { get; set; }
        public string AlternativeEmail { get; set; }
        public string Department { get; set; }
        public string UserId { get; set; }
        public DateTime SentAt { get; set; }
    }
}
