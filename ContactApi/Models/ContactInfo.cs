using Entities.Models;

namespace ContactApi.Models
{
    public class ContactInfoPoco
    {
        public Guid ContactId { get; set; }
        public ContactInfo.InfoType ContactInformationType { get; set; }
        public string Content { get; set; }
    }
}
