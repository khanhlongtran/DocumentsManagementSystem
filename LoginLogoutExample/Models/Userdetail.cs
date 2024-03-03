using System;
using System.Collections.Generic;

namespace LoginLogoutExample.Models
{
    public partial class Userdetail
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Mobile { get; set; } = null!;

    }
}
