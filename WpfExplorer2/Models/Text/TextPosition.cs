using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfExplorer.Models.Text
{
    public class TextPosition : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private int _row;
        private int _column;

        private string _rowcolumn;

        public TextPosition()
        {
            _row = 1;
            _column = 1;
            _rowcolumn = "1:1";
        }

        public int Row { get { return _row; } set { _row = value; } }

        public int Column { get { return _column; } set { _column = value; } }

        public string PositionStr { get { return _rowcolumn; } set { _rowcolumn = value; } }

        public void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
