using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using DatingAPI.Data;
using DatingAPI.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace DatingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController:ControllerBase
    {
           private readonly IDatingRepository _DatingRepository;
            private readonly IMapper _mapper;
        
        public UsersController(IDatingRepository  DatingRepository,IMapper mapper)
        {
                _DatingRepository=DatingRepository;  
                _mapper = mapper;         
        }

        [HttpGet("GetUsers")]
        public async Task<IActionResult> getUsers()
        {
            var users = await _DatingRepository.GetUsers();

            var userCollectons = _mapper.Map<IEnumerable<UsersForDto>>(users);

            return Ok(userCollectons);
        }

        
        [HttpGet("UserbyID")]
        public async Task<IActionResult> getUsers(int id)
        {
            var user = await _DatingRepository.GetUser(id);

           var userToReturn = _mapper.Map<UserDetailDto>(user);

            return Ok(userToReturn);
        }
    }
}