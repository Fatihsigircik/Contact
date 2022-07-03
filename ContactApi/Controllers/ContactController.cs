using ContactApi.Models;
using DbContext;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ContactApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactController : ControllerBase
    {
        private readonly ContactContext _context;

        public ContactController(DbContext.ContactContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Route("")]
        public async Task<ActionResult<Guid>> Post([FromBody] ContactPoco request)
        {
            var contact = new Entities.Models.Contact(request.FirstName, request.LastName, request.CompanyName);
            await _context.Contacts.AddAsync(contact);
            await _context.SaveChangesAsync();
            return Ok(contact.Id);
        }


        [HttpDelete]
        [Route("")]
        public async Task<ActionResult<Guid>> Delete([FromBody] DeleteObject request)
        {
            var contact = await _context.Contacts.FirstOrDefaultAsync(m => m.Id == request.Id);
            if (contact is null)
            {
                return BadRequest();
            }

            _context.Contacts.Remove(contact);
            await _context.SaveChangesAsync();
            return Ok(contact.Id);
        }

        [HttpGet]
        [Route("")]
        public async Task<ActionResult<List<Entities.Models.Contact>>> GetAll()
        {
            var list = await _context.Contacts.Include(q=>q.ContactInfos).ToListAsync();
            return Ok(list);
        }

        [HttpGet]
        [Route("{contactId:guid}")]
        public async Task<ActionResult<Entities.Models.Contact>> GetByContactId([FromRoute] Guid contactId)
        {
            var item = await _context.Contacts.Include(q=>q.ContactInfos).FirstOrDefaultAsync(q => q.Id == contactId);
            if (item == null)
            {
                return NotFound(contactId);
            }
            return Ok(item);
        }
    }
}
