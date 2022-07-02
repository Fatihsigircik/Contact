using Entities.Abstract;
using Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    public class Contact : BaseEntity
    {

        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string CompanyName { get; private set; }

        public Contact()
        {

        }

        public Contact(string firstName, string lastName, string companyName = null)
        {
            FirstName = Utility.CheckNullOrEmpty(firstName, nameof(firstName));
            LastName = Utility.CheckNullOrEmpty(lastName, nameof(firstName)); ;
            CompanyName = companyName;
        }
        
        
        public ICollection<ContactInfo> ContactInfos { get; set; } = new List<ContactInfo>();
        public IReadOnlyCollection<ContactInfo> PhoneInfos => ContactInfos.Where(m => m.InformationType == ContactInfo.InfoType.Phone).ToList();
        public IReadOnlyCollection<ContactInfo> EmailInfos => ContactInfos.Where(m => m.InformationType == ContactInfo.InfoType.Email).ToList();
        public IReadOnlyCollection<ContactInfo> LocationInfos => ContactInfos.Where(m => m.InformationType == ContactInfo.InfoType.Location).ToList();

    }
}
