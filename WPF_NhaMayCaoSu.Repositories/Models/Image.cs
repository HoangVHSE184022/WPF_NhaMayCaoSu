
namespace WPF_NhaMayCaoSu.Repository.Models
{
    public class Image
    {
        public Guid ImageId { get; set; }
        public short ImageType { get; set; }
        public string ImagePath { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public Guid SaleId { get; set; }
        public Sale Sale { get; set; }
    }

}
