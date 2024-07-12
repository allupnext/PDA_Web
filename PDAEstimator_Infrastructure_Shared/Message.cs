using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDAEstimator_Infrastructure_Shared
{
    public class Message
    {
        public List<MailboxAddress> To { get; set; }
        public List<MailboxAddress> Cc { get; set; }
        public string Subject { get; set; }
        public string FromCompany { get; set; } 
        public string Content { get; set; }
        public Message(IEnumerable<string> to, IEnumerable<string> cc, string subject, string content, string fromCompany)
        {
            To = new List<MailboxAddress>();
            To.AddRange(to.Select(x => new MailboxAddress("email", x)));
            Cc = new List<MailboxAddress>();
            Cc.AddRange(cc.Select(x => new MailboxAddress("email", x)));
            Subject = subject;
            Content = content;
            FromCompany = fromCompany;
        }   
    }
}
