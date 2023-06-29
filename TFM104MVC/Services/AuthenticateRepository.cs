using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TFM104MVC.Database;
using TFM104MVC.Models;
using TFM104MVC.Models.Entity;

namespace TFM104MVC.Services
{
    public class AuthenticateRepository : IAuthenticateRepository
    {
        private AppDbContext _context;
        public AuthenticateRepository(AppDbContext appDbContext)
        {
            _context = appDbContext;
        }

        public User AccountCheck(string account)
        {
            return _context.Users.FirstOrDefault(x => x.Account == account);
        }

        public void AddUser(User user)
        {
            _context.Users.Add(user);
        }

        public User CheckUser(string account, string password)
        {
            return _context.Users.FirstOrDefault(x => x.Account == account && x.Password == password);
        }

        public User FindUser(int userId)
        {
            return _context.Users.Include(x => x.Members).FirstOrDefault(x => x.Id == userId);
        }

        public bool Save()
        {
            return (_context.SaveChanges() >= 0);
        }

        public User FindFirm(int userId)
        {
            return _context.Users.Include(x => x.Firms).FirstOrDefault(x => x.Id == userId);
        }

        public User FindTheOnlyUser(int userId)
        {
            return _context.Users.FirstOrDefault(x => x.Id == userId);
        }
        public User FindUserPic(int userId)
        {
            return _context.Users.Include(x=>x.Members).FirstOrDefault(x => x.Id == userId);
        }

        public User FindAdminOrFirmUser(int userId)
        {
            return _context.Users.Include(x=>x.Firms).Include(x=>x.Admins).FirstOrDefault(x=>x.Id == userId);
        }

        public User GetUserByPassword(string password)
        {
            return _context.Users.FirstOrDefault(x => x.Salt == password);
        }
    }
}
