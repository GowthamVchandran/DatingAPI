using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingAPI.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DatingAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly DataContext _context;
        public ValuesController(DataContext context)
        {
            _context = context;

        }
        // GET api/values
        [AllowAnonymous]
        [HttpGet("GetTest")]                
        public async Task<IActionResult> Get()
        {
            var result = await _context.UserValues.ToListAsync();

            return Ok(result);
        }


        // GET api/values/5
        [AllowAnonymous]
        [HttpGet("Testid")]        
        public async Task<IActionResult> Get(int id)
        {
            var getDetails = await _context.UserValues.FirstOrDefaultAsync(x => x.Id == id);

            if (getDetails == null)
                return BadRequest("Record doesn't exists");

            return Ok(getDetails);

        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
