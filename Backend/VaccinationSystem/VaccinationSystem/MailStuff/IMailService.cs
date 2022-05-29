using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace VaccinationSystem.MailStuff
{
    public interface IMailService
    {
        Task SendEmailAsync(MailRequest mailRequest);
    }
}
