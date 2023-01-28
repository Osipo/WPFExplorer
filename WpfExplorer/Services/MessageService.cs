using DevExpress.Mvvm.Native;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfExplorer.Models;
using WpfExplorer.ViewModels;

namespace WpfExplorer.Services
{
    public class MessageService
    {
        private readonly ConcurrentDictionary<MessageSubscriber, Func<IMessage, Task>> _consumers;

        public MessageService() {
            _consumers = new ConcurrentDictionary<MessageSubscriber, Func<IMessage, Task>>();
        }

        public async Task SendTo(IMessage message, Guid? reciever)
        {
            var messageType = message.GetType();
            
            var tasks = _consumers
                .Where(s => s.Key.MessageType == messageType && s.Key.RecieverGuid == reciever)
                .Select(s => s.Value(message));

            await Task.WhenAll(tasks);
        }

        public async Task SendTo(IMessage message, IEnumerable<Guid?> recievers)
        {
            var messageType = message.GetType();
            var tasks = _consumers
                .Where(s => s.Key.MessageType == messageType && recievers.Contains(s.Key.RecieverGuid))
                .Select(s => s.Value(message));
            await Task.WhenAll(tasks);
        }

        public async Task SendToAll(IMessage message)
        {
            var messageType = message.GetType();
            var tasks = _consumers
                .Where(s => s.Key.MessageType == messageType)
                .Select(s => s.Value(message));
            await Task.WhenAll(tasks);
           
        }

        public MessageSubscriber Subscribe<TMessage>(object reciever, Func<TMessage, Task> handler) where TMessage : IMessage
        {
            var sub = new MessageSubscriber(reciever, s => _consumers.TryRemove(s, out var _), typeof(TMessage));

            _consumers.TryAdd(sub, (@event) => handler((TMessage)@event) );

            return sub;
        }

        public int Subscribers
        {
            get { return _consumers.Count; }
        }
    }
}