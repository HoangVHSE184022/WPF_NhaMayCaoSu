namespace WPF_NhaMayCaoSu.Repository.Models
{
    public class Customer
    {
        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string Phone { get; set; }
        public short Status { get; set; }
        public float? bonusPrice { get; set; }
        public ICollection<RFID> RFIDs { get; set; }
        public int RFIDCount => RFIDs?.Count(r => r.Status == 1) ?? 0;
    }

}
