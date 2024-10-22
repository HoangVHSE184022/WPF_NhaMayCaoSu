namespace WPF_NhaMayCaoSu.Repository.Models
{
    public class Sale
    {
        public Guid SaleId { get; set; }
        public string CustomerName { get; set; }
        public float? ProductDensity { get; set; }
        public float? ProductWeight { get; set; }
        public float? TareWeight { get; set; }
        public DateTime? LastEditedTime { get; set; }
        public short Status { get; set; }
        public string? Note { get; set; }
        public float? SalePrice { get; set; }
        public string RFIDCode { get; set; }
        public Guid RFID_Id { get; set; }
        public ICollection<Image> Images { get; set; }
        public RFID RFID { get; set; }
    }
}
