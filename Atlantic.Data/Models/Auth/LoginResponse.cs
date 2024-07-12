using Atlantic.Data.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlantic.Data.Models.Auth
{
    public class LoginResponse
    {
        public string Email { get; set; }
        public string Message { get; set; }
        public string Token { get; set; }
        public UserDto UserDto { get; set; }
    }
}
