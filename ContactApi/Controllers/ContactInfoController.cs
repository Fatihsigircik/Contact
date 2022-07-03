using ContactApi.Models;
using DbContext;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ContactApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]


    public class ContactInfoController : ControllerBase
    {
        private readonly ContactContext _context;
        public ContactInfoController(DbContext.ContactContext context)
        {
            _context = context;
        }


        [HttpPost]
        [Route("")]
        public async Task<ActionResult<Guid>> Post([FromBody] ContactInfoPoco request)
        {

            var contact = await _context.Contacts.FirstOrDefaultAsync(q => q.Id == request.ContactId);

            if (contact is null)
            {
                throw new Exception($"Entity \"{nameof(Entities.Models.Contact)}\" ({request.ContactId}) was not found.");
            }

            var contactInfo = new Entities.Models.ContactInfo(request.ContactInformationType, request.Content, contact);
            await _context.ContactInfos.AddAsync(contactInfo);
            await _context.SaveChangesAsync();
            return Ok(contactInfo.Id);
        }


        [HttpDelete]
        [Route("")]
        public async Task<ActionResult<Guid>> Delete([FromBody] DeleteObject request)
        {
            var contactInfo = await _context.ContactInfos.FirstOrDefaultAsync(q => q.Id == request.Id);
            if (contactInfo is null)
            {
                return BadRequest();
            }

            _context.ContactInfos.Remove(contactInfo);
            await _context.SaveChangesAsync();
            return Ok(contactInfo.Id);
        }


    }
}
