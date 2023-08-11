using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace WpfExplorer
{
    public static class BindingErrors
    {

#if !DEBUG
    public static void Hide() { }
    public static void Show() { }
#endif

#if DEBUG
        private static SourceLevels _defaultLevel = PresentationTraceSources.DataBindingSource.Switch.Level;

        private static bool _isHiding = false;

        public static void Hide()
        {
            if (!_isHiding)
            {
                _isHiding = true;
                _defaultLevel = PresentationTraceSources.DataBindingSource.Switch.Level;
                PresentationTraceSources.DataBindingSource.Switch.Level = SourceLevels.Critical;
            }
        }

        public static void Show()
        {
            if (_isHiding)
            {
                /// Wait for UI to load before showing binding errors again
                App.Current.Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action(() =>
                {
                    _isHiding = false;
                    PresentationTraceSources.DataBindingSource.Switch.Level = _defaultLevel;
                    
                }));
            }
        }
#endif
    }
}
