using AutoMapper.Execution;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TFM104MVC.Database;
using TFM104MVC.Models.Entity;
using TFM104MVC.Extensions;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Net.Http;
using Newtonsoft.Json;
using TFM104MVC.Services;
using System.Web;

namespace TFM104MVC.Controllers
{
    public class LoginController : Controller
    {
        private AppDbContext _context;
        string _redirect_uri = "https://tibame4mvc.azurewebsites.net/Login/UseLineLogin";
        string _client_id = "2000010936";
        string _state = "123";
        string _client_serect = "af9f062cfb3a83216fb5044a042caa8e";

        private readonly IHttpClientFactory _clientFactory;
        private readonly IAuthenticateRepository _authenticateRepository;
        public LoginController(AppDbContext appDbContext, IHttpClientFactory httpClientFactory, IAuthenticateRepository authenticateRepository)
        {
            _context = appDbContext;
            _clientFactory = httpClientFactory;
            _authenticateRepository = authenticateRepository;
        }

        //private readonly IConfiguration _configuration;
        //private readonly IAuthenticateRepository _authenticateRepository;
        //private readonly IMapper _mapper;
        //private readonly ISender _sender;
        //private readonly IProductRepository _productRepository;
        //private readonly IHttpContextAccessor _httpContextAccessor;
        //public LoginController(IConfiguration configuration, IAuthenticateRepository authenticateRepository, IMapper mapper, ISender sender, IProductRepository productRepository, IHttpContextAccessor httpContextAccessor)

        //{
        //    _configuration = configuration;
        //    _authenticateRepository = authenticateRepository;
        //    _mapper = mapper;
        //    _sender = sender;
        //    _productRepository = productRepository;
        //    _httpContextAccessor = httpContextAccessor;

        //}

        //會員管理頁
        [Authorize(AuthenticationSchemes = "Cookies")]
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> ResponseAsync()
        {
            var res = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            var data = res.Principal.Claims.Select(x => new KeyValuePair<string, string>(x.Type, x.Value)).ToList();
            var email = data.GetValueByKey("emailaddress");
            var lastName = data.GetValueByKey("surname");
            var firstName = data.GetValueByKey("givenname");
            if (string.IsNullOrWhiteSpace(email)) throw new System.Exception("Can not Find Fb Email");
            var existUser = _context.Users.FirstOrDefault(x => x.Account == email);
            if (existUser == null)
            {
                var member = new User
                {
                    Account = email,
                    Password = "zzzzzzzz",
                    LastName = lastName,
                    FirstName = firstName,
                    RoleName = "Member"
                };

                _context.Users.Add(member);
                _context.SaveChanges();
            };
            var saveUserAlready = _authenticateRepository.AccountCheck(email);
            var claims = new[]
            {
                new Claim(ClaimTypes.Email,email),
                new Claim("email",email),
                new Claim("userId",saveUserAlready.Id.ToString()),
                new Claim(ClaimTypes.Name,lastName+firstName),
                new Claim(ClaimTypes.Role,saveUserAlready.RoleName)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            await HttpContext.SignInAsync(claimsPrincipal);

            return Redirect("~/Home/Index");
        }




        /// <summary>
        /// FB登入(修)
        /// </summary>
        /// <returns></returns>
        public IActionResult FbLogin()
        {
            var p = new AuthenticationProperties()
            {
                RedirectUri = Url.Action("Response")
            };
            return Challenge(p, FacebookDefaults.AuthenticationScheme);
        }
        /// <summary>
        /// Google登入(修)
        /// </summary>
        /// <returns></returns>
        public IActionResult GoogleLogin()
        {
            var g = new AuthenticationProperties()
            {
                RedirectUri = Url.Action("Response")
            };
            return Challenge(g, GoogleDefaults.AuthenticationScheme);
        }

        [HttpPost]
        public IActionResult LineLogin()
        {
            return Ok($"https://access.line.me/oauth2/v2.1/authorize?response_type=code&client_id={_client_id}&redirect_uri={_redirect_uri}&state={_state}&scope=profile%20openid%20email");

        }


        public async Task<IActionResult> UseLineLogin(string code, string state, string error, string error_description)
        {
            if (!string.IsNullOrEmpty(error) || _state != state || string.IsNullOrEmpty(code))
                return RedirectToAction(nameof(Index));

            var url = "https://api.line.me/oauth2/v2.1/token";
            var posData = new Dictionary<string, string>()
            {
                {"client_id",_client_id},
                {"client_secret",_client_serect},
                {"code",code},
                {"grant_type","authorization_code"},
                {"redirect_uri","https://" + HttpContext.Request.Host.ToString() + "/Login/UseLineLogin"},
                { "Content-Type",":application/x-www-form-urlencoded"}
            };

            var contentPost = new FormUrlEncodedContent(posData);
            var client = _clientFactory.CreateClient();
            var response = await client.PostAsync(url, contentPost);

            string responseContent;
            if (response.IsSuccessStatusCode)
                responseContent = await response.Content.ReadAsStringAsync();
            else
                return RedirectToAction(nameof(Index));

            var lineLoginResource = JsonConvert.DeserializeObject<LINELoginResource>(responseContent);

            url = $"https://api.line.me/oauth2/v2.1/verify";
            string Token = $"id_token={lineLoginResource.IDToken}&client_id={_client_id}";
            var nvc = HttpUtility.ParseQueryString(Token);
            var dictionaryToken = nvc.AllKeys.ToDictionary(k => k, k => nvc[k]);
            var p2 = new FormUrlEncodedContent(dictionaryToken);
            var responseUserInfo = await client.PostAsync(url, p2);
            string responseContent1;
            if (responseUserInfo.IsSuccessStatusCode)
            {
                responseContent1 = await responseUserInfo.Content.ReadAsStringAsync();
                var lineLoginUserResource = JsonConvert.DeserializeObject<LINEUser>(responseContent1);
                var existUser = _authenticateRepository.AccountCheck(lineLoginUserResource.UserEmail);
                if (existUser == null)
                {
                    Models.Entity.Member memberData = new Models.Entity.Member();
                    memberData.PicPath = lineLoginUserResource.PictureUrl;
                    var member = new User
                    {
                        Account = lineLoginUserResource.UserEmail,
                        Password = "bbbbbb",
                        LastName = lineLoginUserResource.Name,
                        FirstName = "",
                        RoleName = "Member",
                        Members = memberData

                    };

                    _authenticateRepository.AddUser(member);
                    _authenticateRepository.Save();
                }
                var saveUserAlready = _authenticateRepository.AccountCheck(lineLoginUserResource.UserEmail);
                var claims = new[]
                {
                new Claim(ClaimTypes.Email,lineLoginUserResource.UserEmail),
                new Claim("email",lineLoginUserResource.UserEmail),
                new Claim("userId",saveUserAlready.Id.ToString()),
                new Claim(ClaimTypes.Name,lineLoginUserResource.Name),
                new Claim(ClaimTypes.Role,saveUserAlready.RoleName)
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                await HttpContext.SignInAsync(claimsPrincipal);


                return Redirect("~/home/index");
            }
            else
            {
                return RedirectToAction(nameof(Index));
            }


            // 2. https://developers.line.biz/en/reference/social-api/#profile
            //url = $"https://api.line.me/v2/profile";
            //client.DefaultRequestHeaders.Add("authorization", $"Bearer {lineLoginResource.AccessToken}");
            //response = await client.GetAsync(url);
            //if (response.IsSuccessStatusCode)
            //{
            //    responseContent = await response.Content.ReadAsStringAsync();
            //    var user = JsonConvert.DeserializeObject<LINEUser>(responseContent);

            //    var existUser = _authenticateRepository.AccountCheck(user.Id);
            //    if (existUser == null)
            //    {
            //        var member = new User
            //        {
            //            Account = user.Id,
            //            Password = "bbbbbb",
            //            LastName = user.Name,
            //            FirstName = "",
            //            RoleName = "Member"
            //        };

            //        _authenticateRepository.AddUser(member);
            //        _authenticateRepository.Save();
            //    };
            //}
            //return Redirect("~/home/index");
        }

        public class LINELoginResource
        {
            [JsonProperty("access_token")]
            public string AccessToken { get; set; }

            [JsonProperty("token_type")]
            public string TokenType { get; set; }

            [JsonProperty("expires_in")]
            public string ExpiresIn { get; set; }

            [JsonProperty("scope")]
            public string Scope { get; set; }

            [JsonProperty("refresh_token")]
            public string RefreshToken { get; set; }

            // 這邊跟一般的TokenResponse不同，多了使用者的Id Token
            [JsonProperty("id_token")]
            public string IDToken { get; set; }
        }

        public class LINEUser
        {

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("picture")]
            public string PictureUrl { get; set; }

            [JsonProperty("email")]
            public string UserEmail { get; set; }
        }
        public IActionResult FindPassword([FromQuery] string reset)
        {
            return View();
        }

    }
}
