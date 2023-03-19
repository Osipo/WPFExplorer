using DevExpress.Mvvm.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfExplorer.Models.Utils;

namespace WpfExplorer.Models.Markov
{
    public class MarkovModel
    {
        private int _size;
        private int _prefixLength;
        private State[] _statetab;

        private static string NONWORD = "\n";

        public MarkovModel(int size, int prefixLength)
        {
            _size = size;
            _statetab = new State[size];
            _prefixLength = prefixLength;
        }

        private State Lookup(string[] prefix, int create)
        {
            int i, h;
            State r = null;
            h = StringUtils.GetHash(prefix, _size);

            for(r = _statetab[h]; r != null; r = r.Next)
            {
               if (StringUtils.Mathes(prefix, r.Prefix))
                   return r;
            }


            if(create == 1)
            {
                r = new State(_prefixLength);
                for (int pi = 0; pi < r.PrefixSize; pi++)
                    r.Prefix[pi] = prefix[pi];
                r.Next = _statetab[h];
                _statetab[h] = r;
            }

            return r;
        }

        public void Init(string text)
        {
            string[] lines = text.Split(new char[] {'\n', '\r'}, StringSplitOptions.RemoveEmptyEntries);
            string[] prefix = new string[_prefixLength];
            int i = 0;
            for (i = 0; i < prefix.Length; i++)
                prefix[i] = NONWORD;

            for(i = 0; i < lines.Length; i++)
            {
                lines[i].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ForEach(x => Add(prefix, x));
            }
            Add(prefix, NONWORD);
        }

        public string Generate(int nwords)
        {
            StringBuilder sb = new StringBuilder();
            Random rand = new Random();

            State sp = null;
            Suffix suf = null;
            string[] prefix = new string[_prefixLength];
            string word = null;
            int i = 0, nmatch = 0;

            for(i = 0; i < _prefixLength; i++)
            {
                prefix[i] = NONWORD;
            }
            for(i = 0; i < nwords; i++)
            {
                sp = Lookup(prefix, 0);
                nmatch = 0;
                for (suf = sp.Suffix; suf != null; suf = suf.Next)
                    if (rand.Next(Int32.MaxValue) % ++nmatch == 0)
                        word = suf.Word;

                if (word != null && word.Equals(NONWORD))
                    break;
                if (word != null)
                {
                    sb.Append(word).Append(' ');
                    Array.Copy(prefix, 1, prefix, 0, prefix.Length - 1);
                    prefix[prefix.Length - 1] = word;
                }
            }
            return sb.ToString();
        }

        public void Add(string[] prefix, string suffix)
        {
            State sp = Lookup(prefix, 1);
            AddSuffix(sp, suffix);
            Array.Copy(prefix, 1, prefix, 0, prefix.Length - 1); //shift one left.
            prefix[prefix.Length - 1] = suffix;
        }
        private void AddSuffix(State sp, string suffix)
        {
            Suffix s = new Suffix(suffix);
            s.Next = sp.Suffix;
            sp.Suffix = s;
        }
    }
}
