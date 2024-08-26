using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_NhaMayCaoSu.Repository.Models
{
    public class RFID
    {
        public Guid RFID_Id { get; set; }
        public long RFIDCode { get; set; }
        public DateTime CreatedDate { get; set; }
        public short Status { get; set; }
        public Guid AccountId { get; set; }

        public Account Account { get; set; }
        public ICollection<Sale> Sales { get; set; }
    }
}
