﻿
namespace WPF_NhaMayCaoSu.Repository.Models
{

    public class RFID
    {
        public Guid RFID_Id { get; set; }
        public string RFIDCode { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public Guid CustomerId { get; set; }
        public short Status { get; set; }
        public Customer Customer { get; set; }
        public ICollection<Sale> Sales { get; set; }
    }

}
