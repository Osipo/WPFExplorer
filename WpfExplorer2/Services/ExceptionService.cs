using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfExplorer.Models;

namespace WpfExplorer.Services
{
    public class ExceptionService
    {

        private readonly MessageService _messageService;
        private Guid _id;
        public ExceptionService(MessageService messageService)
        {
            _messageService = messageService;
            _id = Guid.NewGuid();

            var sub = _messageService.Subscribe<ExceptionMessage>(this, async msg => {
                await Task.Delay(1);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(msg);
                Console.ForegroundColor = ConsoleColor.Black;
            });
        }

        public Guid Guid { get { return _id; } }

        public async Task Notify(string msg, Exception e)
        {
            await _messageService.SendTo(new ExceptionMessage(msg, e), this);
        }
    }
}
