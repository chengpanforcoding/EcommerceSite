using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TFM104MVC.Services
{
    public interface ISender
    {
        void Sender(string email, string subject, string message);
    }
}
