﻿namespace WPF_NhaMayCaoSu.Repository.Models
{
    public class Sale
    {
        public Guid SaleId { get; set; }
        public double? ProductDensity { get; set; }
        public string DensityImageUrl { get; set; }
        public DateTime CreatedDate { get; set; }
        public short Status { get; set; }
        public bool IsEdited { get; set; }
        public DateTime? LastEditedTime { get; set; }
        public long RFIDCode { get; set; }

        public Customer Customer { get; set; }
    }
}