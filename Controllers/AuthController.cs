using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DatingAPI.Data;
using DatingAPI.Dtos;
using DatingAPI.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DatingAPI.Controllers
{
    
    [Route("api/[Controller]/[action]")]
    [ApiController]
    public class AuthController:ControllerBase
    {
        private readonly IAuthRepository _AuthRepository;

         private readonly IMapper _mapper;
        private readonly IConfiguration _config;
        public AuthController(IAuthRepository  AuthRepository,IConfiguration config,IMapper mapper)
        {
                _AuthRepository = AuthRepository;
                _config = config;
                _mapper = mapper;
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(LoginDto login)
        {
         
            var UserLogin = await _AuthRepository.Login(login.UserName,login.Password);

             if(UserLogin == null)
              throw new Exception("Please provide valid credentials");;

            string _token=  CreateToken(UserLogin);

            var user = _mapper.Map<UserDetailDto>(UserLogin);

              return Ok( new {
                  token = _token,user
              });
    
         }
        public string CreateToken(User UserLogin)
        {
          var claims = new []
              {
                new Claim(ClaimTypes.NameIdentifier,UserLogin.Id.ToString()),
                new Claim(ClaimTypes.Name,UserLogin.UserName)
              };

              var key= new SymmetricSecurityKey(
                  Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));

              var credential= new SigningCredentials(key,SecurityAlgorithms.HmacSha512Signature);

              var tokenDescription= new SecurityTokenDescriptor
              {
                  Subject= new ClaimsIdentity(claims),
                  Expires= DateTime.Now.AddHours(2.00),
                  SigningCredentials= credential
              };

              var  tokenHandler = new JwtSecurityTokenHandler();
              var token = tokenHandler.CreateToken(tokenDescription);

          return tokenHandler.WriteToken(token);
        }
        
        [HttpPost]
        public async Task<IActionResult> Register(RegisterDto Register){
                 
            Register.Username=Register.Username.ToLower();

            if( await _AuthRepository.IsExists(Register.Username))
              throw new Exception("User already exists");

            var userNameDto= _mapper.Map<User>(Register);
            

           var CreatedUser = await _AuthRepository.Register(userNameDto,Register.Password);

            var userToReturn = _mapper.Map<UserDetailDto>(CreatedUser);
            
            return  CreatedAtRoute("getUserbyID",new {Controller="Users",id = CreatedUser.Id},userToReturn);
        }
    }
}