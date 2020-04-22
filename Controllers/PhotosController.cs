using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using DatingAPI.Controllers.Helper;
using DatingAPI.Data;
using DatingAPI.Dtos;
using DatingAPI.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DatingAPI.Controllers
{
    [Authorize]
    [Route("api/users/{userId}/photos")]
    [ApiController]
    public class PhotosController:ControllerBase
    {
       private readonly IDatingRepository _DatingRepository;
            private readonly IMapper _mapper;
            private readonly IOptions<CloudinarySettings> _cloudinaryConfig;

            private Cloudinary _cloudinary;

        public PhotosController(IDatingRepository  DaingRepository,IMapper mapper,IOptions<CloudinarySettings> cloudConfig)
        {
                _DatingRepository= DaingRepository;
                _mapper= mapper;
                _cloudinaryConfig = cloudConfig;

                Account acc = new Account(_cloudinaryConfig.Value.CloudName,_cloudinaryConfig.Value.ApiKey,_cloudinaryConfig.Value.ApiSecret );

                _cloudinary = new Cloudinary(acc);
        }

        [HttpGet("{id}",Name="GetPhoto")]
        public async Task<IActionResult> GetPhoto(int id)
        {
              var photoFromRepo =  await  _DatingRepository.GetPhoto(id);

              var photo = _mapper.Map<PhotoForReturnDto>(photoFromRepo);

              return Ok(photo);
        }
     
      
        [HttpPost]
        public async Task<IActionResult> AddPhoto(int userId, [FromForm]photoForCreatingDto photoForDto){

            if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) 
            return Unauthorized();

            var userRepositoy = await _DatingRepository.GetUser(userId);

            var file= photoForDto.File;

            var uploadCouldResult = new ImageUploadResult();

            if(file.Length>0){

                using(var stream = file.OpenReadStream())
                {
                    var uploadParams= new ImageUploadParams(){
                        File = new FileDescription(file.Name,stream),
                        Transformation = new Transformation().Width(500).Height(500).Crop("fill").Gravity("face")
                    };

                    uploadCouldResult = _cloudinary.Upload(uploadParams);
                }
            }
            photoForDto.Url= uploadCouldResult.Uri.ToString();
            photoForDto.PublicId=uploadCouldResult.PublicId;

            var photo = _mapper.Map<Photo>(photoForDto);

            if(!userRepositoy.Photos.Any(x => x.IsMain))
              photo.IsMain = true;

              userRepositoy.Photos.Add(photo);

              if(await _DatingRepository.SaveAll())
              {
                  
                  var photoToReturn = _mapper.Map<PhotoForReturnDto>(photo);
                  return CreatedAtRoute("GetPhoto", new {id= photo.Id},photoToReturn);
              }

            return BadRequest("Could not add photos");
        }

        [HttpPost("{id}/setMain")]
        public async Task<IActionResult> setMainPhoto(int userId,int id)
        {
            if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) 
            return Unauthorized();

            var user = await _DatingRepository.GetUser(userId);

            if(user.Photos.Any(x =>x.Id == id))
            {
                var photoFromRepo = await _DatingRepository.GetPhoto(id);

                if(photoFromRepo.IsMain)
                  return BadRequest("Already has main photo");

                var currentMainPhoto = await _DatingRepository.GetMainPhoto(userId);
                currentMainPhoto.IsMain = false;

                photoFromRepo.IsMain = true;

                if(await _DatingRepository.SaveAll()){
                    return NoContent();
                }
                
                return BadRequest("Could not set photo as Main");
            }
            return Unauthorized();
        }

         [HttpDelete("{id}")]
        public async Task<IActionResult> deletePhoto(int userId,int id)
        {
            if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) 
            return Unauthorized();

            var user = await _DatingRepository.GetUser(userId);

            if(!user.Photos.Any(x =>x.Id == id))
              return Unauthorized();

                var photoFromRepo = await _DatingRepository.GetPhoto(id);

                if(photoFromRepo.IsMain)
                  return BadRequest("you can't delete main photo");

             if(photoFromRepo.PublicId!=null){

              var deleteParams = new DeletionParams(photoFromRepo.PublicId);

              var result = _cloudinary.Destroy(deleteParams);

              if(result.Result == "ok"){
                _DatingRepository.Delete(photoFromRepo);
              }

             }
             if(photoFromRepo.PublicId == null){
                 _DatingRepository.Delete(photoFromRepo);
             }

              if(await _DatingRepository.SaveAll()) return Ok();

              return BadRequest("Failed to delete");
        }
    }
}