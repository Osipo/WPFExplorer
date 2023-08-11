using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfExplorer.Models.ML
{
    public class BPEncoder : ITokenizer
    {
        private Dictionary<string, int> _tokens; //vocabulary
        private Dictionary<(string, string), string> _merges; //Byte-Pair 

        private int _nextId; //num for token in vocabulary
        private int _minSize; //desired size of vocabulary
        private int _offset; //offset from letters and special tokens.

        public BPEncoder(int minSize) {
            _tokens = new Dictionary<string, int>();
            _merges = new Dictionary<(string, string), string> ();
            _nextId = 0;
            _minSize = minSize;
        }

        public int VocabularyMinCapacity { get { return _minSize; } }

        public void Flush()
        {
            _merges.Clear();
            _tokens.Clear();
            _nextId = 0;
            _offset = 0;
        }

        public IEnumerable<string> Tokenize(string content)
        {
            //Pre-Tokenization.
            return Tokenize(content.Split(new[] { ' ', '\f', '\t', '\r', '\n', '\v' }, StringSplitOptions.RemoveEmptyEntries));
        }

        private IEnumerable<string> Tokenize(IEnumerable<string> words)
        {
            if (_tokens.Count == 0)
                throw new InvalidOperationException("Cannot tokenize without dictionary!\n Train model first by call Init(training_content) to get dictionary of tokens!");


            //Save words and their letters
            List<IEnumerable<string>> splits = new List<IEnumerable<string>>();
            foreach (var word in words)
            {
                splits.Add(word.ToList().Select(x => x.ToString())); //IEnumerable<char> -> IEnumerable<string> {single character}
            }

            int idx = 0, i = 0;
            bool isAdded = true;
            foreach (KeyValuePair<(string, string), string> kv in _merges)
            {
                for (idx = 0; idx < splits.Count(); idx++)
                {
                    var split = splits[idx];
                    i = 0;
                    isAdded = true;
                    while (i < split.Count())
                    {
                        if (split.ElementAt(i) == kv.Key.Item1 && split.ElementAt(i + 1) == kv.Key.Item2)
                        {
                            split = split.Take(i).Union(new[] { kv.Value }).Union(split.Skip(i + 2));
                            isAdded = false; //at least one merging was performed => is not new word.
                        }
                        else
                            i++;
                    }
                    if (isAdded)
                    {
                        _tokens.Add(split.Aggregate(string.Empty, (a, b) => a + b), _nextId++); //new word append to dictionary.
                    }
                    splits[idx] = split;
                }
            }
            return splits.Select(x => x.Aggregate(string.Empty, (a, b) => a + b));
        }

        public IEnumerable<int> EncodedTokens(string content)
        {
            return Tokenize(content).Select(x => _tokens[x]);
        }

        public IEnumerable<int> EncodedTokens(IEnumerable<string> words)
        {
            //Content passed to Tokenize(str) method to apply Normalization of the content.
            return Tokenize(words.Aggregate(string.Empty, (a, b) => a + ' ' + b)).Select(x => _tokens[x]);
        }

        public IEnumerable<string> DecodedTokens(IEnumerable<int> encoded)
        {
            return _tokens.AsEnumerable().Join(encoded, (x) => x.Value, y => y, (x, y) => x.Key);
        }

        public void Init(string content)
        {
            Init(content.Split(new[] {' ', '\f', '\t', '\r', '\n', '\v'}, StringSplitOptions.RemoveEmptyEntries));
        }

        public void Init(IEnumerable<string> words)
        {
            //Add special token into vocabulary fisrt.
            //From GPT-2
            _tokens.Add("<|endoftext|>", _nextId++);
            _offset = 1;

            //Count frequencies of words.
            Counter<string> counter = new Counter<string>(words);
            List<(string, int)> freqs = counter.Frequencies();

            //Split words into alphas.
            SortedSet<char> alphas = new SortedSet<char>();
            foreach(string word in words)
                foreach(char c in word)
                    alphas.Add(c);

            //Add alphabet into vocabulary (signle letters)
            foreach(char c in alphas)
            {
                _tokens.Add(c + "", _nextId++);
                _offset++;
            }

            //Save words and their letters
            Dictionary<string, IEnumerable<string>> word_symbols = new Dictionary<string, IEnumerable<string>>();
            foreach(var (item, freq) in freqs){
                word_symbols.Add(item, item.ToList().Select(x => x.ToString()));
            }


            while(_tokens.Count < _minSize)
            {
                var pair_freqs = Compute_Pair_Freqs(freqs, word_symbols);
                (string, string) best_pair = ("", "");
                var max_freq = 0;
                foreach(KeyValuePair<(string, string),int> kv in pair_freqs)
                {
                    if(max_freq < kv.Value)
                    {
                        max_freq = kv.Value;
                        best_pair = kv.Key;
                    }

                }
                word_symbols = Merge_Pair(freqs, word_symbols, best_pair.Item1, best_pair.Item2);
                _merges[best_pair] = best_pair.Item1 + best_pair.Item2;
                _tokens.Add(best_pair.Item1 + best_pair.Item2, _nextId++);
            }
        }

        private Dictionary<(string, string), int> Compute_Pair_Freqs(List<(string, int)> freqs, Dictionary<string, IEnumerable<string>> word_symbols)
        {
            Dictionary<(string, string), int> pair_freqs = new Dictionary<(string, string), int>();
            foreach(var (item, freq) in freqs)
            {
                var split = word_symbols[item];
                if (split.Count() == 1)
                    continue;
                for(int i = 0; i < split.Count() - 1; i++)
                {
                    var pair = (split.ElementAt(i), split.ElementAt(i + 1));
                    if(pair_freqs.ContainsKey(pair))
                        pair_freqs[pair] += freq;
                    else
                        pair_freqs.Add(pair, freq);
                }
            }

            return pair_freqs;
        }

        private Dictionary<string, IEnumerable<string>> Merge_Pair(List<(string, int)> freqs, Dictionary<string, IEnumerable<string>> splits, string a, string b)
        {
            int i = 0;
            foreach (var (item, freq) in freqs)
            {
                var split = splits[item];
                
                if (split.Count() == 1)
                    continue;

                i = 0;
                while(i < split.Count() - 1)
                {
                    if (split.ElementAt(i) == a && split.ElementAt(i + 1) == b)
                    {
                        split = split.Take(i).Union(new[] { a, b }).Union(split.Skip(i + 2));
                    }
                    else
                        i++;
                }
                splits[item] = split;
            }
            return splits;
        }
    }
}