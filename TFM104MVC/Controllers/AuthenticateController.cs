using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TFM104MVC.Dtos;
using TFM104MVC.Models;
using TFM104MVC.Models.Entity;
using TFM104MVC.Services;

namespace TFM104MVC.Controllers
{

    [ApiController]
    [Route("{auth}")]
    public class AuthenticateController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IAuthenticateRepository _authenticateRepository;
        private readonly IMapper _mapper;
        private readonly ISender _sender;
        private readonly IProductRepository _productRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AuthenticateController(IConfiguration configuration, IAuthenticateRepository authenticateRepository, IMapper mapper, ISender sender, IProductRepository productRepository, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _authenticateRepository = authenticateRepository;
            _mapper = mapper;
            _sender = sender;
            _productRepository = productRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> loginAsync([FromBody] LoginDto loginDto)
        {
            // 1.驗證帳號與密碼正確與否
            //先驗證帳號 看有沒有此帳號存在 如果沒有 返回帳號密碼錯誤
            //若成功 則得到資料庫裡面這位User的資料 就可以把他的密碼和鹽拿出來做處理
            var loginUser = _authenticateRepository.AccountCheck(loginDto.Account);

            //var loginUser = _authenticateRepository.CheckUser(loginDto.Account, loginDto.Password);
            if (loginUser == null)
            {
                return NotFound("帳號或密碼錯誤");
            }
            string salt = loginUser.Salt;
            byte[] passwordAndSaltBytes = Encoding.UTF8.GetBytes(loginDto.Password + salt);
            byte[] hashBytes = new SHA256Managed().ComputeHash(passwordAndSaltBytes);
            string hashStr = Convert.ToBase64String(hashBytes);
            //驗證密碼
            var loginPasswordCheck = _authenticateRepository.CheckUser(loginDto.Account, hashStr);
            if (loginPasswordCheck == null)
            {
                return NotFound("帳號密碼錯誤");
            }

            //給cookie與session
            var claims = new[]
            {
                new Claim(ClaimTypes.Email,loginPasswordCheck.Account),
                new Claim("email",loginPasswordCheck.Account),
                new Claim("userId",loginPasswordCheck.Id.ToString()),
                new Claim(ClaimTypes.Name,loginPasswordCheck.LastName),
                new Claim(ClaimTypes.Role,loginPasswordCheck.RoleName)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            await HttpContext.SignInAsync(claimsPrincipal);



            return Ok("登入成功");
            //// 2.若正確 則創建JWT Token
            //// header (SHA256加密Header)
            //var signinAlgorithm = SecurityAlgorithms.HmacSha256;
            //// payload
            //var claims = new[]
            //{
            //    // sub
            //    new Claim(JwtRegisteredClaimNames.Sub,loginUser.Id.ToString()),
            //    new Claim(ClaimTypes.Role,loginUser.RoleName),
            //    new Claim("userId",loginUser.Id.ToString())
            //};
            //// signiture
            //var secretByte = Encoding.UTF8.GetBytes(_configuration["Authentication:SecretKey"]);
            //var signinKey = new SymmetricSecurityKey(secretByte);
            //var signinCredentials = new SigningCredentials(signinKey, signinAlgorithm);

            //var token = new JwtSecurityToken(
            //    issuer: _configuration["Authentication:Issuer"] ,
            //    audience: _configuration["Authentication:Audience"],
            //    claims,
            //    notBefore:DateTime.UtcNow,
            //    expires:DateTime.UtcNow.AddDays(1),
            //    signinCredentials
            //    );
            //var tokenStr = new JwtSecurityTokenHandler().WriteToken(token);
            //// 3.return 200 ok + jwt
            //return Ok(tokenStr);
        }


        [HttpPost("logout")]
        [Authorize(AuthenticationSchemes = "Cookies")]
        public IActionResult logoutAsync()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return Ok("登出成功");

        }


        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> register([FromBody] UserForCreationDto userForCreationDto)
        {
            var userModel = _mapper.Map<User>(userForCreationDto);

            string salt = Guid.NewGuid().ToString();
            string password = userForCreationDto.Password;
            byte[] passwordWithSaltBytes = Encoding.UTF8.GetBytes(password + salt);
            byte[] hashByte = new SHA256Managed().ComputeHash(passwordWithSaltBytes);
            string hashStr = Convert.ToBase64String(hashByte);

            userModel.Password = hashStr;
            userModel.Salt = salt;

            if (userModel.RoleName == "Member")
            {
                userModel.LastName = "Guest";
                userModel.Members.PicPath = "https://fakeimg.pl/50/";
            }

            //string userName = User.Identity.Name;
            //userName = "Guest";

            var accountCheck = _authenticateRepository.AccountCheck(userForCreationDto.Account);
            if (accountCheck != null)
            {
                return NotFound("帳號重複 請更換帳號名稱");
            }
            _authenticateRepository.AddUser(userModel);
            _authenticateRepository.Save();

            string Url = "http://tibame4mvc.azurewebsites.net/auth/open?account=" + userForCreationDto.Account; //之後改成發布後的網址
            string messageUrl = $"<a href='{Url}'>我是開通帳號小精靈~~</a>";
            string subject = "註冊驗證信";
            string message = "恭喜註冊成功，請點擊以下文字開通您的帳號" + "<br>" + messageUrl;
            _sender.Sender(userModel.Account, subject, message);

            await _productRepository.SaveAsync();

            return Ok("註冊成功");
        }

        [AllowAnonymous]
        [HttpPost("open")]
        public IActionResult OpenAccount([FromQuery] string account)
        {
            var userFromRepo = _authenticateRepository.AccountCheck(account);

            if (userFromRepo == null)
            {
                throw new ArgumentNullException();
            }

            userFromRepo.Verification = true;
            _authenticateRepository.Save();

            return NoContent();
        }

        [AllowAnonymous]
        [HttpPut("reset")]
        public IActionResult UpdatePassword([FromBody] LoginDto updatePasswordDto)
        {
            var userExist = _authenticateRepository.AccountCheck(updatePasswordDto.Account);
            if (userExist == null)
            {
                return NotFound("無此使用者帳號");
            }
            string oldSalt = userExist.Salt;

            string newPassword = updatePasswordDto.Password;
            byte[] newPasswordWithOldSalt = Encoding.UTF8.GetBytes(newPassword + oldSalt);
            byte[] hashByte = new SHA256Managed().ComputeHash(newPasswordWithOldSalt);
            string hashStr = Convert.ToBase64String(hashByte);
            userExist.Password = hashStr;

            _authenticateRepository.Save();

            return Ok("更新密碼完成，請妥善保管您的密碼");

        }

        [HttpPost("UpdateUserDetail")]
        [Authorize(AuthenticationSchemes = "Cookies")]
        public IActionResult UpdateUserDetail([FromBody] UserForUpdate userForUpdate)
        {
            //取出使用者Id
            var userId = _httpContextAccessor.HttpContext.User.FindFirstValue("userId");
            int UserId = int.Parse(userId);
            //取出這個使用者資料表
            var user = _authenticateRepository.FindUser(UserId);

            //開始修改
            user.FirstName = userForUpdate.FirstName;
            user.LastName = userForUpdate.LastName;
            user.Phone = userForUpdate.Phone;
            user.Members.Gender = userForUpdate.Members.Gender;
            user.Members.Birthday = Convert.ToDateTime(userForUpdate.Members.Birthday);

            //儲存
            _authenticateRepository.Save();
            return NoContent();
        }

        [HttpGet("getUser")]
        public IActionResult GetUserById()
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirstValue("userId");
            var Id = int.Parse(userId);
            var userFromRepo = _authenticateRepository.FindUser(Id);
            if (userFromRepo == null)
            {
                Console.WriteLine($"找不到編號為{Id}的使用者");
                return NotFound($"找不到編號為{Id}的使用者");
            }
            var userDto = _mapper.Map<UserMemberDto>(userFromRepo);
            return Ok(userDto);
        }

        [HttpGet("getFirm")] //撈廠商資料渲染在後台管理中心
        public IActionResult GetFirmById()
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirstValue("userId");
            var Id = int.Parse(userId);
            var userFromRepo = _authenticateRepository.FindFirm(Id);
            if (userFromRepo == null)
            {
                Console.WriteLine($"找不到編號為{Id}的使用者");
                return NotFound($"找不到編號為{Id}的使用者");
            }
            var userDto = _mapper.Map<UserFirmDto>(userFromRepo);
            return Ok(userDto);
        }

        [HttpPost("UpdateFirmDetail")]
        [Authorize(AuthenticationSchemes = "Cookies")]
        public IActionResult UpdateFirmDetail([FromBody] UserFirmDto firmForUpdate)
        {
            //取出使用者Id
            var userId = _httpContextAccessor.HttpContext.User.FindFirstValue("userId");
            int UserId = int.Parse(userId);
            //取出這個使用者資料表
            var user = _authenticateRepository.FindFirm(UserId);

            //開始修改
            user.FirstName = firmForUpdate.FirstName;
            user.LastName = firmForUpdate.LastName;
            user.Phone = firmForUpdate.Phone;
            user.Firms.TaxId = firmForUpdate.Firms.TaxId;
            user.Firms.Name = firmForUpdate.Firms.Name;

            //儲存
            _authenticateRepository.Save();
            return NoContent();
        }

        [HttpPost("ConfirmPassword")]
        [Authorize(AuthenticationSchemes = "Cookies")]
        public IActionResult ConfirmPassword([FromQuery] string passwordNow)
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirstValue("userId");
            var Id = int.Parse(userId);
            var user = _authenticateRepository.FindTheOnlyUser(Id);
            string userSalt = user.Salt;
            byte[] userPasswordByte = Encoding.UTF8.GetBytes(passwordNow + userSalt);
            byte[] userSHA256Password = new SHA256Managed().ComputeHash(userPasswordByte);
            string userSHA256PasswordStr = Convert.ToBase64String(userSHA256Password);

            if (user.Password != userSHA256PasswordStr)
            {
                return NotFound("密碼錯誤");
            }

            return NoContent();
        }

        [HttpPost("UpdateFirmPassword")]
        [Authorize(AuthenticationSchemes = "Cookies")]
        public IActionResult UpdateFirmPassword([FromQuery] string passwordNew)
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirstValue("userId");
            var Id = int.Parse(userId);
            var user = _authenticateRepository.FindTheOnlyUser(Id);
            string userSalt = user.Salt;

            string newPassword = passwordNew;
            byte[] newPasswordWithOldSalt = Encoding.UTF8.GetBytes(newPassword + userSalt);
            byte[] hashByte = new SHA256Managed().ComputeHash(newPasswordWithOldSalt);
            string hashStr = Convert.ToBase64String(hashByte);
            user.Password = hashStr;

            _authenticateRepository.Save();

            return Ok("更新密碼完成，請妥善保管您的密碼");
        }

        [HttpPost("checkAccount")] //確認帳號存在與否
        public IActionResult checkAccount([FromQuery] string account)
        {
            var email = _authenticateRepository.AccountCheck(account);

            //var loginUser = _authenticateRepository.CheckUser(loginDto.Account, loginDto.Password);
            if (email != null)
            {
                return Ok("帳號已存在");
            }
            return Ok("帳號可使用");
        }

        [HttpGet("GetUserPic")]
        [Authorize(AuthenticationSchemes = "Cookies")]
        public IActionResult GetUserPic()
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirstValue("userId");
            var Id = int.Parse(userId);
            var userJudge = _authenticateRepository.FindAdminOrFirmUser(Id);
            if (userJudge.Firms ==null || userJudge.Admins == null)
            {
                var user = _authenticateRepository.FindUserPic(Id);
                string userPic = user.Members.PicPath;
                if (userPic == null)
                {
                    return Ok("https://fakeimg.pl/50/");
                }
                return Ok(userPic);
            }

            return Ok("https://fakeimg.pl/50/");
        }


        [HttpPost("forgetpassword")]
        public IActionResult ForgetPassword([FromQuery] string account)
        {
            var userExist = _authenticateRepository.AccountCheck(account);
            if (userExist == null)
            {
                return NotFound("沒有此帳號"); 
            }
            string Url = "http://tibame4mvc.azurewebsites.net/Login/FindPassword?password=" + userExist.Salt + "," + userExist.Account; //之後改成發布後的網址
            string messageUrl = $"<a href='{Url}'>我是找回密碼小精靈~~</a>";
            string subject = "忘記密碼驗證信";
            string message = "請點擊以下文字更新您的密碼，並妥善保管" + "<br>" + messageUrl;
            _sender.Sender(userExist.Account, subject, message);

            return Ok("請至信箱收取重設密碼驗證信");
        }

        [HttpPost("resetpassword")]
        public IActionResult ResetPassword([FromQuery] string password,string account,string newpassword)
        {
            var userAccount = _authenticateRepository.AccountCheck(account);
            if(userAccount == null)
            {
                return NotFound("沒有此帳號");
            }

            var userPassword = _authenticateRepository.GetUserByPassword(password);
            if(userPassword == null)
            {
                return NotFound("仿造密碼是不對的");
            }
            if(userPassword.Id != userAccount.Id)
            {
                return NotFound("你是仿造密碼的人");
            }

            string salt = userAccount.Salt;
            byte[] passwordWithSaltBytes = Encoding.UTF8.GetBytes(newpassword + salt);
            byte[] hashByte = new SHA256Managed().ComputeHash(passwordWithSaltBytes);
            string hashStr = Convert.ToBase64String(hashByte);

            userPassword.Password = hashStr;
            _authenticateRepository.Save();

            return Ok("更新密碼成功 請妥善保管您的新密碼");


        }

    }
}
