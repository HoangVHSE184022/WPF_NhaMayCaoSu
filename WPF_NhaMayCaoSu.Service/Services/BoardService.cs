using WPF_NhaMayCaoSu.Repository.IRepositories;
using WPF_NhaMayCaoSu.Repository.Models;
using WPF_NhaMayCaoSu.Repository.Repositories;
using WPF_NhaMayCaoSu.Service.Interfaces;

namespace WPF_NhaMayCaoSu.Service.Services
{
    public class BoardService : IBoardService
    {
        private readonly IBoardRepository _boardRepository = new BoardRepository();

        public async Task CreateBoardAsync(Board board)
        {
            await _boardRepository.CreateBoardAsync(board);
        }

        public async Task<IEnumerable<Board>> GetAllBoardsAsync(int pageNumber, int pageSize)
        {
            return await _boardRepository.GetAllBoardsAsync(pageNumber, pageSize);
        }

        public async Task<Board> GetBoardByIdAsync(Guid boardId)
        {
            return await _boardRepository.GetBoardByIdAsync(boardId);
        }

        public async Task UpdateBoardAsync(Board board)
        {
            await _boardRepository.UpdateBoardAsync(board);
        }

        public async Task DeleteBoardAsync(Guid boardId)
        {
            await _boardRepository.DeleteBoardAsync(boardId);
        }

        public async Task<Board> GetBoardByNameAsync(string boardName)
        {
            return await _boardRepository.GetBoardByNameAsync(boardName);
        }

        public async Task<int> GetTotalBoardsCountAsync()
        {
            return await _boardRepository.GetTotalBoardsCountAsync();
        }

        public async Task<Board> GetBoardByMacAddressAsync(String BoardMacAddress)
        {
            return await _boardRepository.GetBoardByMacAddressAsync(BoardMacAddress);
        }
    }
}
