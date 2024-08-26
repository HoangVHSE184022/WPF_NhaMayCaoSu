﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_NhaMayCaoSu.Repository.Models
{
    public class Account
    {
        public Guid AccountId { get; set; }
        public string AccountName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public DateTime CreatedDate { get; set; }
        public long Status { get; set; }
        public Guid RoleId { get; set; }

        public Role Role { get; set; }

        public ICollection<RFID> RFIDs { get; set; }
    }
}