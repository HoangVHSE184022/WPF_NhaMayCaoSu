namespace WPF_NhaMayCaoSu.Repository.Models
{
    public class Customer
    {
        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string RFIDCode { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public short Status { get; set; }
        public ICollection<Sale> Sales { get; set; }
    }
}
