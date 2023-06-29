using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TFM104MVC.Models;
using TFM104MVC.Models.Entity;

namespace TFM104MVC.Services
{
    public interface IAuthenticateRepository
    {

        User CheckUser(string account , string password);
        User AccountCheck(string account);

        void AddUser(User user);

        bool Save();

        User FindUser(int userId);

        User FindFirm(int userId);
        User FindTheOnlyUser(int userId);
        User FindUserPic(int userId);
        User FindAdminOrFirmUser(int userId);
        User GetUserByPassword(string password);
    }
}
