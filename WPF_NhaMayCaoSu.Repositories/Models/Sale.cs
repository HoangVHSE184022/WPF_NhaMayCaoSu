using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_NhaMayCaoSu.Repository.Models
{
    public class Sale
    {
        public Guid SaleId { get; set; }
        public string EmployeeName { get; set; }
        public double? ProductWeight { get; set; }
        public double? ProductDensity { get; set; }
        public string WeightImgageUrl { get; set; }
        public string DensityImageUrl { get; set; }
        public DateTime CreatedDate { get; set; }
        public short Status { get; set; }
        public short? IsEdited { get; set; }
        public DateTime? LastEditedTime { get; set; }
        public Guid RFID_Id { get; set; }

        public RFID RFID { get; set; }
    }
}
