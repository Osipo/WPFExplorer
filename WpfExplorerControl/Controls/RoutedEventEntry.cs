using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfExplorerControl.Control
{
    public readonly struct RoutedEventEntry
    {
        private readonly RoutedEvent _event;
        private readonly Delegate _handler;
        public RoutedEventEntry(RoutedEvent e, Delegate handler)
        {
            _event = e;
            _handler = handler;
        }

        public Delegate Handler { get { return _handler; } }
        public RoutedEvent Event { get { return _event; } }

        public override bool Equals(object obj)
        {
            if(obj == null || obj.GetType() != typeof(RoutedEventEntry)) return false;
            return Equals((RoutedEventEntry)obj);
        }
        public bool Equals(RoutedEventEntry b)
        {
            return b.Event == _event && b.Handler == Handler;
        }

        public override int GetHashCode()
        {
            int h = 17;
            h = 31 * h + _event.GetHashCode();
            h = 31 * h + _handler.GetHashCode();
            return h;
        }

        public static bool operator ==(RoutedEventEntry a, RoutedEventEntry b)
        {
            return a.Equals(b);
        }
        public static bool operator !=(RoutedEventEntry a, RoutedEventEntry b)
        {
            return !a.Equals(b);
        }
    }
}
