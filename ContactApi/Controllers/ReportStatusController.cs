using DbContext;
using Entities.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ContactApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportStatusController : ControllerBase
    {
        private readonly ContactContext _context;
        public ReportStatusController(DbContext.ContactContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Route("")]
        public async Task<ActionResult<Guid>> CreateReport()
        {
            var reportStatus = new Entities.Models.ReportStatus(ReportStatus.StatusType.Processing);
            await _context.ReportStatuses.AddAsync(reportStatus);
            await _context.SaveChangesAsync();
            await new KafkaInfrastructure.Producer().PublishMessage("ReportStatus", reportStatus.Id.ToString());
            return Ok(reportStatus.Id);
        }

        [HttpGet]
        [Route("{ReportStatusId:Guid}")]
        public async Task<ActionResult<ReportStatus>> GetById([FromRoute] Guid ReportStatusId)
        {
            var item = await _context.ReportStatuses.FirstOrDefaultAsync(q => q.Id == ReportStatusId);
            if (item == null)
            {
                return NotFound(ReportStatusId);
            }
            return Ok(item);
        }
        [HttpGet]
        public async Task<ActionResult<List<ReportStatus>>> GetAll()
        {
            var list = await _context.ReportStatuses.ToListAsync();
            return Ok(list);
        }
    }
}
