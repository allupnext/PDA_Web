using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDAEstimator_Infrastructure_Shared.Services
{
    public interface IEmailSender
    {
        void SendEmail(Message message, string filepath= "");
    }
}
