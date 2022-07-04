using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DbContext;
using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace KafkaInfrastructure
{
    internal class ReportStatusPrepare
    {
        private readonly ContactContext _context;
        private readonly Guid _reportGuid;

        public ReportStatusPrepare(DbContext.ContactContext context,Guid reportGuid)
        {
            _context = context;
            _reportGuid = reportGuid;
        }
        internal async Task<bool> PrepareReport()
        {
            try
            {
                var contactCountByLocation = await _context.ContactInfos
                    .Where(m => m.InformationType == ContactInfo.InfoType.Location)
                    .GroupBy(m => new
                    {
                        m.Content,
                        m.Contact.Id
                    }).Select(m => new
                    {
                        ContentName = m.Key.Content,
                    }).ToListAsync();

                var contactInfoLocations = contactCountByLocation
                    .GroupBy(m => m.ContentName)
                    .Select(m => new ContactInformationType
                    {
                        ContentName = m.Key,
                        Count = m.Count(),
                        PhoneNumberCount =
                            _context.Contacts
                                .Where(y =>
                                    y.ContactInfos.Any(x => x.InformationType == ContactInfo.InfoType.Location && x.Content == m.Key))
                                .Include(x => x.ContactInfos)
                                .Select(x => x.ContactInfos.Where(m => m.InformationType == ContactInfo.InfoType.Phone)).ToList().SelectMany(x => x).ToList().Count()
                    }).ToList();



                saveReportFile(contactInfoLocations);
                await setDocumentLogStatusAsCompleted();

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
            
        }


        private async Task setDocumentLogStatusAsCompleted()
        {
            var reportStatus = await _context.ReportStatuses.FirstOrDefaultAsync(q => q.Id == _reportGuid);
            reportStatus.Status = ReportStatus.StatusType.Completed;
            await _context.SaveChangesAsync();

        }

        private void saveReportFile(List<ContactInformationType> contactInfos)
        {
           StringBuilder sb = new StringBuilder();

            sb.AppendLine("Lokasyon\t\t\tKişi Sayısı\t\t\tTelefon Numarası Sayısı");

            foreach (var stat in contactInfos)
            {
                sb.AppendLine($"{stat.ContentName}\t\t\t{stat.Count}\t\t\t\t{stat.PhoneNumberCount}");
            }

            using FileStream fs = new(Path.Combine(Directory.GetCurrentDirectory(), "ReportFiles", _reportGuid + ".txt"),FileMode.Append);
            using StreamWriter sw = new(fs);
            sw.Write(sb);
            sw.Flush();
        }

         private class ContactInformationType
         {
             public string ContentName { get; set; }
             public int Count { get; set; }
             public int PhoneNumberCount { get; set; }

         }
    }


}
