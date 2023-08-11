using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace WpfExplorer.Models.ML
{
    public class WordsInfo : INotifyPropertyChanged
    {
        private int _minWordLen;
        private int _maxWordLen;
        private int _wordCount;

        public WordsInfo() {
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        
        public int MinWordLength { get { return _minWordLen; } }

        public int MaxWordLength { get { return _maxWordLen; } }

        public int WordCount { get { return _wordCount; } }



        public void NotifyAboutContent(string content)
        {
            if (String.IsNullOrWhiteSpace(content))
            {
                _wordCount = 0;
                _minWordLen = 0;
                _maxWordLen = 0;
            }
            else
            {
                var ws = content.Split(new[] {'\n', ' ', '\t', '\r', '\f', '\v'}, StringSplitOptions.RemoveEmptyEntries);
                if(ws != null && ws.Length > 0)
                {
                    _wordCount = ws.Length;
                    var l = ws.Aggregate(string.Empty, (seed, f) => (f?.Length ?? 0) > seed.Length ? f : seed);
                    _maxWordLen = l.Length;
                    var s = ws.Aggregate(l, (seed, f) => (f?.Length ?? 0) < seed.Length ? f : seed);
                    _minWordLen = s.Length;
                }
            }

            OnPropertyChanged("WordCount");
            OnPropertyChanged("MinWordLength");
            OnPropertyChanged("MaxWordLength");
        }
    }
}
