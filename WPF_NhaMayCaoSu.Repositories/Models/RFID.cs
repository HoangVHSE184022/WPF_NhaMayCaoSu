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
        public string RFIDCode { get; set; }
        public DateTime ExpirationDate { get; set; }
        public Guid CustomerId { get; set; }
        public short Status { get; set; }

        // Navigation properties
        public Customer Customer { get; set; }
    }

}
