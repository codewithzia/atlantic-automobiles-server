using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlantic.Data.Models.Requests
{
    public class SuperAdminRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }

    }
}
