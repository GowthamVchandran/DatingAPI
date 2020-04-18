using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DatingAPI.Data;
using DatingAPI.Dtos;
using DatingAPI.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DatingAPI.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController:ControllerBase
    {
        private readonly IAuthRepository _AuthRepository;
        private readonly IConfiguration _config;
        public AuthController(IAuthRepository  AuthRepository,IConfiguration config)
        {
                _AuthRepository=AuthRepository;
                _config=config;
        }
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto login)
        {
          //  try
           // { 
             
              
            var UserLogin = await _AuthRepository.Login(login.UserName,login.Password);

             if(UserLogin == null)
              throw new Exception("User Already exits");;

            string _token=  CreateToken(UserLogin);

              return Ok( new {
                  token = _token
              });
           // }
            // catch(Exception)
            // {
            //    return StatusCode(501);
            // }
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
                  Expires= DateTime.Now.AddDays(1),
                  SigningCredentials= credential
              };

              var  tokenHandler = new JwtSecurityTokenHandler();
              var token = tokenHandler.CreateToken(tokenDescription);

          return tokenHandler.WriteToken(token);
        }
        
        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterDto user){
                 
            user.Username=user.Username.ToLower();

            if( await _AuthRepository.IsExists(user.Username))
              throw new Exception("User already exists");

            var userNameDto=new User{
                UserName=user.Username
            };  

              await _AuthRepository.Register(userNameDto,user.Password);
            
            return StatusCode(201);
        }
    }
}