using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF_NhaMayCaoSu.Repository.Context;
using WPF_NhaMayCaoSu.Repository.IRepositories;

namespace WPF_NhaMayCaoSu.Repository.Repositories
{
    public class ImageRepository : IImageRepository
    {
        private CaoSuWpfDbContext _context;
    }
}
