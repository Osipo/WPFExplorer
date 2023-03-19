using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfExplorer.Models.Markov
{
    public class Suffix
    {
        private string _word;

        private Suffix _next;

        public Suffix(string word) {
            _word = word;
        }



        public Suffix Next { get { return _next; } set { _next = value; } }

        public string Word { get { return _word; } }
    }
}
