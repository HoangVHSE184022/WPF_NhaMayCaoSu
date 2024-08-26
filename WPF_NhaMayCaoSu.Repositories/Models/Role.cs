using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_NhaMayCaoSu.Repository.Models
{
    public class Role
    {
        public Guid RoleId { get; set; }
        public string RoleName { get; set; }

        public ICollection<Account> Accounts { get; set; }
    }
}
