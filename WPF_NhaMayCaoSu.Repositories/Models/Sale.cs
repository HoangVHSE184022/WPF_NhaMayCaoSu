namespace WPF_NhaMayCaoSu.Repository.Models
{
    public class Sale
    {
        public Guid SaleId { get; set; }
        public double? ProductDensity { get; set; }
        public string? DensityImageUrl { get; set; }
        public double? ProductWeight { get; set; }
        public string? WeightImageUrl { get; set; }
        public DateTime CreatedDate { get; set; }
        public short Status { get; set; }
        public bool IsEdited { get; set; }
        public DateTime? LastEditedTime { get; set; }
        public string RFIDCode { get; set; }
        public Customer Customer { get; set; }
        public short Type { get; set; }
    }
}
