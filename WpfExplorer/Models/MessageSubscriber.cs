using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfExplorer.ViewModels;

namespace WpfExplorer.Models
{
    public class MessageSubscriber : IDisposable
    {
        private readonly Action<MessageSubscriber> _action;


        public Type MessageType { get; }
        public Guid? RecieverGuid { get; }

        public Guid? SenderGuid { get; }

        public MessageSubscriber(Guid? senderGuid, Guid? recieverGuid, Action<MessageSubscriber> action, Type messageType)
        {
            _action = action;
            RecieverGuid = recieverGuid;
            SenderGuid = senderGuid;
            MessageType = messageType;
        }

        public MessageSubscriber(object sender, object reciever, Action<MessageSubscriber> action, Type messageType) 
            : this(ExtractGuidFromVM(sender), ExtractGuidFromVM(reciever), action, messageType)
        {

        }

        public MessageSubscriber(Action<MessageSubscriber> action, Type messageType) : this(null, null, action, messageType)  
        {
        }
        public MessageSubscriber(Guid? recieverGuid, Action<MessageSubscriber> action, Type messageType) : this(null, recieverGuid, action, messageType)
        {
        }

        public MessageSubscriber(object reciever, Action<MessageSubscriber> action, Type messageType) : this(ExtractGuidFromVM(reciever), action, messageType)
        {

        }

        public void Dispose()
        {
            _action?.Invoke(this);
        }

        public static Guid? ExtractGuidFromVM(object reciever)
        {
            Guid? r = null;
            if(
                reciever != null && 
                (reciever.GetType() == typeof(ViewModelBaseWithGuid) || reciever.GetType().IsSubclassOf(typeof(ViewModelBaseWithGuid)))
            ) {
                r = (reciever as ViewModelBaseWithGuid).Guid;
            }
            return r;
        }
    }
}
