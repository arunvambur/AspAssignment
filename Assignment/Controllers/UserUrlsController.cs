using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Assignment.Data;

namespace Assignment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserUrlsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UserUrlsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/UserUrls
        [HttpGet]
        public IEnumerable<UserUrl> GetUserUrl()
        {
            return _context.UserUrl;
        }

        // GET: api/UserUrls/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserUrl([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userUrl = await _context.UserUrl.FindAsync(id);

            if (userUrl == null)
            {
                return NotFound();
            }

            return Ok(userUrl);
        }

        // PUT: api/UserUrls/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUserUrl([FromRoute] string id, [FromBody] UserUrl userUrl)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != userUrl.UrlId)
            {
                return BadRequest();
            }

            _context.Entry(userUrl).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserUrlExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/UserUrls
        [HttpPost]
        public async Task<IActionResult> PostUserUrl([FromBody] UserUrl userUrl)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.UserUrl.Add(userUrl);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUserUrl", new { id = userUrl.UrlId }, userUrl);
        }

        // DELETE: api/UserUrls/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserUrl([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userUrl = await _context.UserUrl.FindAsync(id);
            if (userUrl == null)
            {
                return NotFound();
            }

            _context.UserUrl.Remove(userUrl);
            await _context.SaveChangesAsync();

            return Ok(userUrl);
        }

        private bool UserUrlExists(string id)
        {
            return _context.UserUrl.Any(e => e.UrlId == id);
        }
    }
}