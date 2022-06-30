using Entities.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    public class ReportStatus:BaseEntity
    {
        public StatusType Status { get; set; }
        public ReportStatus(StatusType status)
        {
            Status = Enum.IsDefined(typeof(StatusType), status)?status: throw new ArgumentOutOfRangeException(nameof(status));
        }
       
        
        public enum StatusType
        {
            Undefine,
            Processing,
            Completed
        }
    }
}
