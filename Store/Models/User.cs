﻿using System;
using System.Collections.Generic;

namespace Store.Models
{
    public class User
    {
        public int Id { get; set; }

        public string Email { get; set; } = null!;

        public string Password { get; set; } = null!;

        public byte[]? Salt { get; set; }
    }
}
