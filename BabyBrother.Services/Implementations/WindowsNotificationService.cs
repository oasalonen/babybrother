using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace BabyBrother.Services.Implementations
{
    public class WindowsNotificationService : INotificationService
    {
        public async Task ShowBlockingMessageAsync(string content, string title)
        {
            var dialog = new MessageDialog(content, title);
            await dialog.ShowAsync();
        }
    }
}
