using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BabyBrother.Services
{
    public interface INotificationService
    {
        Task ShowBlockingMessageAsync(string content, string title);
    }
}
