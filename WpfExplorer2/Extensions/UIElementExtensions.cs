using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WpfExplorerControl.Control;
using BF = System.Reflection.BindingFlags;

namespace WpfExplorerControl.Extensions
{
    public static class UIElementExtensions
    {
        public static RoutedEventHandlerInfo[] GetRoutedEventHandlers(this UIElement element, RoutedEvent e)
        {
            PropertyInfo eventHandlersStoreProperty = typeof(UIElement).GetProperty("EventHandlersStore", BF.Instance | BF.NonPublic);
            object eventHandlersStore = eventHandlersStoreProperty.GetValue(element, null); //System.Windows.EventHandlersStore
            if (eventHandlersStore == null)
                return null;
            Console.WriteLine(e);
            PropertyInfo pcnt = eventHandlersStore.GetType().GetProperty("Count", BF.Instance | BF.NonPublic | BF.Public);
            Console.WriteLine(pcnt.GetValue(eventHandlersStore));

            MethodInfo getRoutedEventHandlers = eventHandlersStore.GetType().GetMethod("GetRoutedEventHandlers", BF.Instance | BF.Public | BF.NonPublic);
            return (RoutedEventHandlerInfo[])getRoutedEventHandlers.Invoke(eventHandlersStore, new object[] { e });
        }


        public static T GetTemplatedChild<T>(this System.Windows.Controls.Control c, string name) where T : DependencyObject
        {
            
            MethodInfo method = c.GetType().GetMethod("GetTemplateChild", BF.Instance | BF.NonPublic);
            return (T)(method?.Invoke(c, new object[] { name }));
        }

        public static object GetVisualParent(this System.Windows.UIElement element)
        {
            PropertyInfo p = element.GetType().GetProperty("VisualParent", BF.Instance | BF.NonPublic);
            return (object)p.GetValue(element);
        }

        public static List<RoutedEventEntry> GetRoutedEventEntries(this UIElement element)
        {
            List<RoutedEventEntry> result = new List<RoutedEventEntry>();
            IEnumerable<FieldInfo> fields = element
                .GetType()
                .GetFields(BF.Static | BF.NonPublic | BF.Instance | BF.Public | BF.FlattenHierarchy)
                .Where(x => x.FieldType == typeof(RoutedEvent));

            foreach (FieldInfo field in fields)
            {
                RoutedEventHandlerInfo[] routedEventHandlerInfos = GetRoutedEventHandlers(element, (RoutedEvent)field.GetValue(element));
                if (routedEventHandlerInfos == null)
                    continue;
                foreach(RoutedEventHandlerInfo info in routedEventHandlerInfos)
                    result.Add(new RoutedEventEntry((RoutedEvent)field.GetValue(element), info.Handler));
            }
            return result.Count > 0 ? result : null;
        }

        public static List<RoutedEventHandlerInfo> GetRoutedEventHandlerInfos(this UIElement element)
        {
            List<RoutedEventHandlerInfo> result = new List<RoutedEventHandlerInfo>();
            IEnumerable<FieldInfo> fields = element
                .GetType()
                .GetFields(BF.Static | BF.NonPublic | BF.Instance | BF.Public | BF.FlattenHierarchy)
                .Where(x => x.FieldType == typeof(RoutedEvent));
            foreach (FieldInfo field in fields)
            {
                RoutedEventHandlerInfo[] routedEventHandlerInfos = GetRoutedEventHandlers(element, (RoutedEvent)field.GetValue(element));
                if (routedEventHandlerInfos != null)
                {
                    result.AddRange(routedEventHandlerInfos);
                }
            }
            return result.Count > 0 ? result : null;
        }

        public static void ClearEventHandlers(this UIElement element)
        {
            int dels = 0;
            int evs = 0;
            List<RoutedEventHandlerInfo> result = new List<RoutedEventHandlerInfo>();
            IEnumerable<FieldInfo> fields = element
                .GetType()
                .GetFields(BF.Static | BF.NonPublic | BF.Instance | BF.Public | BF.FlattenHierarchy)
                .Where(x => x.FieldType == typeof(RoutedEvent));
            foreach (FieldInfo field in fields)
            {
                evs++;
                RoutedEventHandlerInfo[] routedEventHandlerInfos = GetRoutedEventHandlers(element, (RoutedEvent)field.GetValue(element));
                if (routedEventHandlerInfos == null)
                    continue;
                foreach(RoutedEventHandlerInfo hndlr in routedEventHandlerInfos)
                {
                    dels++;
                    element.RemoveHandler((RoutedEvent)field.GetValue(element), hndlr.Handler);
                }
            }
            Console.WriteLine("deleted handlers count " + dels + " of events " + evs);
        }
    }
}
