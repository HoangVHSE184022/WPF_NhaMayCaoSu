using WPF_NhaMayCaoSu.Repository.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WPF_NhaMayCaoSu.Service.Interfaces
{
    public interface IBoardService
    {
        Task CreateBoardAsync(Board board);
        Task<IEnumerable<Board>> GetAllBoardsAsync(int pageNumber, int pageSize);
        Task<Board> GetBoardByIdAsync(Guid boardId);
        Task UpdateBoardAsync(Board board);
        Task DeleteBoardAsync(Guid boardId);
        Task<Board> GetBoardByNameAsync(string boardName);
        Task<int> GetTotalBoardsCountAsync();
        Task<Board> GetBoardByMacAddressAsync(String BoardMacAddress);
    }
}
