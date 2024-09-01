namespace WPF_NhaMayCaoSu.Repository.Models
{
    public class Customer
    {
        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string Phone { get; set; }
        public short Status { get; set; }
        public ICollection<RFID> RFIDs { get; set; }
        public int RFIDCount => RFIDs?.Count ?? 0; //đếm RFID
    }

}
