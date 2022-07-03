using Entities.Abstract;
using Entities.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Entities.Models
{
    public class ContactInfo: BaseEntity
    {

        public InfoType InformationType { get;  set; }

        public string Content { get; set; }

        public Contact Contact { get; set; }

        public ContactInfo()
        { }

        public ContactInfo(InfoType informationType, string content, Contact contact)
        {
      
            InformationType = Enum.IsDefined(typeof(InfoType), informationType)?informationType: throw new ArgumentOutOfRangeException(nameof(informationType));
            Content = Utility.CheckNullOrEmpty(content,nameof(content));
            Contact = contact?? throw new NullReferenceException(nameof(contact));
        }
        
        public enum InfoType
        {           
            Undefine,
            Phone,
            Email,
            Location
        }
    }
}
