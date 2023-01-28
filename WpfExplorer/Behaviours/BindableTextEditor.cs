using ICSharpCode.AvalonEdit;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfExplorer.Behaviours
{
    public class BindableTextEditor : TextEditor, INotifyPropertyChanged
    {


        public static readonly DependencyProperty BTextProperty;        
      
        static BindableTextEditor()
        {
            BTextProperty = DependencyProperty.Register("BText", typeof(string), typeof(BindableTextEditor),
                new FrameworkPropertyMetadata
                {
                     PropertyChangedCallback =
                    (obj, args) =>
                    {
                        BindableTextEditor target = (BindableTextEditor)obj;

                        target.Document.Text = (string)args.NewValue ?? "";
                    },

                     DefaultValue = default(string),
                     BindsTwoWayByDefault = true

                }
            );
        }

        public string BText
        {
            get { return (string)GetValue(BTextProperty); } //base.Text;
            set { SetValue(BTextProperty, value); RaisePropertyChanged("BText"); }
        }

        protected static void OnDependencyPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var target = (BindableTextEditor)obj;

            if (target.Document != null)
            {
                var caretOffset = target.CaretOffset;
                var newValue = args.NewValue;

                if (newValue == null)
                {
                    newValue = "";
                }

                target.Document.Text = (string)newValue;
                target.CaretOffset = Math.Min(caretOffset, newValue.ToString().Length);
            }
        }

        protected override void OnTextChanged(EventArgs e)
        {
            if (this.Document != null)
            {
                //base.Text = this.Document.Text;
                //Console.WriteLine(Document.Text);
            }
            base.OnTextChanged(e);
        }



        /// <summary>
        /// Raises a property changed event
        /// </summary>
        /// <param name="property">The name of the property that updates</param>
        public void RaisePropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
