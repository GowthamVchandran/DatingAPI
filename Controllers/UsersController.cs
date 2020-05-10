using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using DatingAPI.Controllers.Helper;
using DatingAPI.Data;
using DatingAPI.Dtos;
using DatingAPI.Filters;
using DatingAPI.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DatingAPI.Controllers
{
    [ServiceFilter(typeof(logUserActivity))]
    [Route("api/[Controller]/[action]")]
    [ApiController]
    [Authorize]
    public class UsersController:ControllerBase
    {
           private readonly IDatingRepository _DatingRepository;
            private readonly IMapper _mapper;
        
        public UsersController(IDatingRepository  DatingRepository,IMapper mapper)
        {
                _DatingRepository=DatingRepository;  
                _mapper = mapper;
        }

        [HttpGet]   
        public async Task<IActionResult> getUsers([FromQuery]userParams userparams)
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var userFromRepo = await _DatingRepository.GetUser(currentUserId);

            userparams.UserId = currentUserId;

            if(string.IsNullOrEmpty(userparams.Gender))
            {
                userparams.Gender =  userFromRepo.Gender == "male"? "Female": "male";
            }

            var users = await _DatingRepository.GetUsers(userparams);

            var userCollectons = _mapper.Map<IEnumerable<UsersForDto>>(users);

            Response.AddPagination(users.CurrentPage, users.PageSize, 
            users.TotalCount, users.TotalPage);

            return Ok(userCollectons);
        }

        
        [HttpGet("{id}", Name="getUserbyID")]
        public async Task<IActionResult> getUserbyID(int id)
        {
            var user = await _DatingRepository.GetUser(id);

           var userToReturn = _mapper.Map<UserDetailDto>(user);

            return Ok(userToReturn);
        }

        [HttpPut]
        public async Task<IActionResult> updateUser(int id,UserforupdateDto  userDetails)
        {
            var test = User; 

            if(id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) 
            return Unauthorized();

            var userRepositoy = await _DatingRepository.GetUser(id);

            _mapper.Map(userDetails,userRepositoy);

            if(await _DatingRepository.SaveAll())
                 return NoContent();

            throw new Exception($"Error updating  value. {id}");
        }

        [HttpPost("{id}/like/{recipientId}")]

        public async Task<IActionResult> LikeUser(int id, int recipientId)
        {
            if(id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) 
            return Unauthorized();

            var like = await _DatingRepository.GetLike(id,recipientId);

            if(like!=null) return BadRequest("already liked");

            if(await _DatingRepository.GetUser(recipientId) == null) return NotFound();

            like = new Likes
            {
                LikerID = id,
                LikeeID = recipientId
            };

            _DatingRepository.Add<Likes>(like);

            if(await _DatingRepository.SaveAll()) 
              return Ok();

             return BadRequest("unable to like user");
        }
    }
}