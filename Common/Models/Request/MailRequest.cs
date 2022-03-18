using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assessment.Common.Models.Request
{
    public class MailRequest
    {
        [Required]
        public string Email { get; set; }

        
        public string Department { get; set; }
    }
}
