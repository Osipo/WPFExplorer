using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace WpfExplorer.Models.Markov
{
    public class State
    {

        private int _prefixSize;
        private string[] _prefix;

        private State _next;
        private Suffix _suf;

        public State(int prefixSize)
        {
            _prefixSize = prefixSize;
            _prefix = new string[prefixSize];
        }

        public State(string[] prefix)
        {
            _prefix = prefix;
            _prefixSize = prefix.Length;
        }

        public int PrefixSize { get { return _prefixSize; } }

        public string[] Prefix { get { return _prefix; } }
        public State Next { get { return _next; } set { _next = value; } }

        public Suffix Suffix { get { return _suf; } set { _suf = value; } }
    }
}
