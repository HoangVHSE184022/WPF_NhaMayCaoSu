using Microsoft.EntityFrameworkCore;
using WPF_NhaMayCaoSu.Repository.Context;
using WPF_NhaMayCaoSu.Repository.IRepositories;
using WPF_NhaMayCaoSu.Repository.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WPF_NhaMayCaoSu.Repository.Repositories
{
    public class BoardRepository : IBoardRepository
    {
        private CaoSuWpfDbContext _context;

        public async Task CreateBoardAsync(Board board)
        {
            _context = new();
            await _context.Boards.AddAsync(board);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Board>> GetAllBoardsAsync(int pageNumber, int pageSize)
        {
            _context = new();
            return await _context.Boards
                                 .Skip((pageNumber - 1) * pageSize)
                                 .Take(pageSize)
                                 .ToListAsync();
        }

        public async Task<Board> GetBoardByIdAsync(Guid boardId)
        {
            _context = new();
            return await _context.Boards.FirstOrDefaultAsync(x => x.BoardId == boardId);
        }

        public async Task UpdateBoardAsync(Board board)
        {
            _context = new();
            _context.Boards.Update(board);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteBoardAsync(Guid boardId)
        {
            _context = new();
            Board board = await _context.Boards.FirstOrDefaultAsync(x => x.BoardId == boardId);
            if (board != null)
            {
                _context.Boards.Remove(board);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Board> GetBoardByNameAsync(string boardName)
        {
            _context = new();
            return await _context.Boards.FirstOrDefaultAsync(x => x.BoardName == boardName);
        }

        public async Task<int> GetTotalBoardsCountAsync()
        {
            _context = new();
            return await _context.Boards.CountAsync();
        }

        public async Task<Board> GetBoardByMacAddressAsync(string BoardMacAddress)
        {
            _context = new();
            return await _context.Boards.FirstOrDefaultAsync(x => x.BoardMacAddress == BoardMacAddress);
        }
    }
}
