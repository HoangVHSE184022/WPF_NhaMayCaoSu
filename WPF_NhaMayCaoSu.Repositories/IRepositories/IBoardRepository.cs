using WPF_NhaMayCaoSu.Repository.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WPF_NhaMayCaoSu.Repository.IRepositories
{
    public interface IBoardRepository
    {
        Task CreateBoardAsync(Board board);
        Task<IEnumerable<Board>> GetAllBoardsAsync(int pageNumber, int pageSize);
        Task<Board> GetBoardByIdAsync(Guid boardId);
        Task UpdateBoardAsync(Board board);
        Task DeleteBoardAsync(Guid boardId);
        Task<Board> GetBoardByNameAsync(string boardName);
        Task<int> GetTotalBoardsCountAsync();
        Task<Board> GetBoardByMacAddressAsync(string BoardMacAddress);
    }
}
