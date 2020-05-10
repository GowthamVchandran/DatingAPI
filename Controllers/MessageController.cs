using System;
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
    [Route("api/users/{userId}/[Controller]")]
    [ApiController]
    [Authorize]
    public class MessageController:ControllerBase
    {
        private readonly IDatingRepository _DatingRepository;
        private readonly IMapper _mapper;
        public MessageController(IDatingRepository datingRepository,IMapper mapper)
        {
            this._DatingRepository = datingRepository;
            this._mapper = mapper;
        }

        [HttpGet("{id}",Name="GetMessage")]
        public async Task<IActionResult> GetMessage(int userId, int id)
        {
           if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) 
               return Unauthorized();

            var msgRepo = await _DatingRepository.GetMessage(id);

            if(msgRepo==null) return NotFound();

            return Ok(msgRepo);
        }

        [HttpPost]
        public async Task<IActionResult> CreateMessage(int userId, MessageForCreatingDto messageCreatingDto)
        {
               var sender = await _DatingRepository.GetUser(userId);

             if(sender.Id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) 
               return Unauthorized();

               messageCreatingDto.SenderId = userId;

               var recipient = await _DatingRepository.GetUser(messageCreatingDto.RecipientId);

               if(recipient == null) return NotFound("could not found");

               var message = _mapper.Map<Message>(messageCreatingDto);

               _DatingRepository.Add(message);

               

               if(await _DatingRepository.SaveAll()){
                   var MessageToReturn = _mapper.Map<MessageToReturnDto>(message);
                  return CreatedAtRoute("GetMessage",new {id = message.Id},MessageToReturn);
               }

                throw new Exception("Unable to send message");
        }

        [HttpGet]
        public async Task<IActionResult> GetMessageForUser(int userId, 
        [FromQuery]MessageParams messageParams)
        {
            if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) 
               return Unauthorized();

               messageParams.UserId = userId;

             var msgFromRepo =  await _DatingRepository.GetMessageForUser(messageParams);

             var message = _mapper.Map<IEnumerable<MessageToReturnDto>>(msgFromRepo);

             Response.AddPagination(msgFromRepo.CurrentPage,msgFromRepo.PageSize,
             msgFromRepo.TotalCount,msgFromRepo.TotalPage);

             return Ok(message);
        }

         [HttpGet("threads/{recipientId}")]
        public async Task<IActionResult> GetMessages(int userId, int recipientId)
        {
               if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) 
               return Unauthorized();

               var messageFromRepo = await _DatingRepository.GetMessageThread(userId,recipientId);

               var messageThread = _mapper.Map<IEnumerable<MessageToReturnDto>>(messageFromRepo);

               return Ok(messageThread);
        }

        [HttpPost("{id}")]

        public async Task<IActionResult> DeleteMessage(int id,int userId)
        {
             if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) 
               return Unauthorized();

               var messageFromRepo = await _DatingRepository.GetMessage(id);

               if(messageFromRepo.SenderId == userId)
               messageFromRepo.SenderDeleted = true;

                if(messageFromRepo.RecipientId == userId)
               messageFromRepo.RecipientDeleted = true;

               if(messageFromRepo.SenderDeleted && messageFromRepo.RecipientDeleted){
                   _DatingRepository.Delete(messageFromRepo);
               }

               if(await _DatingRepository.SaveAll())
                  return NoContent();

                throw new Exception("unable to delete");
                 
        }
        [HttpPost("{id}/read")]
         public async Task<IActionResult> MarkMessageAsRead(int id,int userId)
         {
               if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) 
               return Unauthorized();

               var message = await _DatingRepository.GetMessage(id);

               if(message.RecipientId!= userId)
                 return Unauthorized();

                message.IsRead = true;
                message.DateRead = DateTime.Now;

                await _DatingRepository.SaveAll();
                
                 return NoContent();
         }


    }
}