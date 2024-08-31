using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_NhaMayCaoSu.Repository.Models
{
    public class Image
    {
        public Guid ImageId { get; set; }
        public long ImageType { get; set; }
        public string ImagePath { get; set; }
        public Guid SaleId { get; set; }

        public Sale Sale { get; set; }
    }

}
