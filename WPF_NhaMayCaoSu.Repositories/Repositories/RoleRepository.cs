﻿using Microsoft.EntityFrameworkCore;
using WPF_NhaMayCaoSu.Repository.Context;
using WPF_NhaMayCaoSu.Repository.IRepositories;
using WPF_NhaMayCaoSu.Repository.Models;

namespace WPF_NhaMayCaoSu.Repository.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private CaoSuWpfDbContext _context;

        public async Task CreateRoleAsync(Role role)
        {
            _context = new();
            await _context.AddAsync(role);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Role>> GetAllRolesAsync(int pageNumber, int pageSize)
        {
            _context = new();

            return await _context.Roles
                                 .Skip((pageNumber - 1) * pageSize)
                                 .Take(pageSize)
                                 .ToListAsync();
        }


        public async Task<Role> GetRoleByIdAsync(Guid roleId)
        {
            _context = new();
            return await _context.Roles.FirstOrDefaultAsync(x => x.RoleId == roleId);
        }

        public async Task UpdateRoleAsync(Role role)
        {
            _context = new();
            _context.Roles.Update(role);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteRoleAsync(Guid roleId)
        {
            _context = new();
            Role role = await _context.Roles.FirstOrDefaultAsync(x => x.RoleId == roleId);
            _context.Remove(role);
            await _context.SaveChangesAsync();
        }
    }
}