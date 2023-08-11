using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfExplorer.Models.ML
{
    public class WordPieceEncoder : ITokenizer
    {
        private Dictionary<string, int> _tokens; //vocabulary

        private int _nextId; //num for token in vocabulary
        private int _minSize; //desired size of vocabulary
        private int _offset; //offset from letters and special tokens.



        public WordPieceEncoder(int minSize)
        {
            _tokens = new Dictionary<string, int>();
            _nextId = 0;
            _minSize = minSize;
        }

        public int VocabularyMinCapacity { get { return _minSize; } }

        public void Flush()
        {
            _tokens.Clear();
            _nextId = 0;
            _offset = 0;
        }

        public IEnumerable<string> Tokenize(string content)
        {

            //Pre-tokenization.
            return Tokenize(content.Split(new[] { ' ', '\f', '\t', '\r', '\n', '\v' }, StringSplitOptions.RemoveEmptyEntries));
        }

        private IEnumerable<string> Tokenize(IEnumerable<string> words)
        {
            if (_tokens.Count == 0)
                throw new InvalidOperationException("Cannot tokenize without dictionary!\n Train model first by call Init(training_content) to get dictionary of tokens!");

            IEnumerable<string> result = new List<string>();
            foreach(var word in words)
            {
                result = result.Union(Word_Tokens(word));
            }
            return result;
        }

        private IEnumerable<string> Word_Tokens(string word)
        {
            List<string> tokens = new List<string>();
            string cword = word; int i = -1;
            while(cword.Length > 0)
            {
                i = cword.Length;
                while (i > 0 && !_tokens.ContainsKey(cword.Take(i).Aggregate(string.Empty, (a, b) => a + b).ToString())) i--;
                if (i == 0)
                {
                    tokens.Add("[UNK]");
                    break;
                }
                tokens.Add(cword.Take(i).Aggregate(string.Empty, (a, b) => a + b).ToString());
                cword = word.Skip(i).Aggregate(string.Empty, (a, b) => a + b).ToString();
                if (cword.Length > 0)
                    cword = "##" + cword;
            }
            return tokens;
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
            

            //Pre-Tokenization.
            Init(content.Split(new[] { ' ', '\f', '\t', '\r', '\n', '\v' }, StringSplitOptions.RemoveEmptyEntries));
        }

        private void Init(IEnumerable<string> words)
        {

            //Add special token into vocabulary fisrt.
            //From GPT-2
            _tokens.Add("<|endoftext|>", _nextId++);
            _offset = 1;

            //Count frequencies of words.
            Counter<string> counter = new Counter<string>(words);
            List<(string, int)> freqs = counter.Frequencies();

            //Append to alphabet first letters and BERT parts ## (rest part of the word)
            SortedSet<string> alphas = new SortedSet<string>();
            foreach (var (item, freq) in freqs)
            {
                alphas.Add(item[0].ToString());
                for(int i = 1; i < item.Length; i++)
                {
                    alphas.Add("##" + item[i]);
                }
            }

            //Add Categories into vocabulary
            _tokens.Add("[PAD]", _nextId++); _offset++;
            _tokens.Add("[UNK]", _nextId++); _offset++;
            _tokens.Add("[CLS]", _nextId++); _offset++;
            _tokens.Add("[SEP]", _nextId++); _offset++;
            _tokens.Add("[MASK]", _nextId++); _offset++;

            //Add alphabet into vocabulary (signle letters)
            foreach (string c in alphas)
            {
                _tokens.Add(c, _nextId++);
                _offset++;
            }

            //Save words and their letters
            Dictionary<string, IEnumerable<string>> word_symbols = new Dictionary<string, IEnumerable<string>>();
            foreach (var (item, freq) in freqs)
            {
                word_symbols.Add(item, new List<string>(){ item[0].ToString() }); //first symbol
                for (int i = 1; i < item.Length; i++)
                {
                    word_symbols[item] = word_symbols[item].Union(item.ToList().Skip(1).Select(x => "##" + x)); //rest part of the word
                }
            }

            while (_tokens.Count < _minSize)
            {
                var pair_freqs = Compute_Pair_Score(freqs, word_symbols);
                (string, string) best_pair = ("", "");
                var max_freq = 0;
                foreach (KeyValuePair<(string, string), int> kv in pair_freqs)
                {
                    if (max_freq < kv.Value)
                    {
                        max_freq = kv.Value;
                        best_pair = kv.Key;
                    }

                }
                word_symbols = Merge_Pair(freqs, word_symbols, best_pair.Item1, best_pair.Item2);
                
                //Remove '##' from second mergin part.
                var ntok = (best_pair.Item2.StartsWith("##")) ? 
                    best_pair.Item1 + best_pair.Item2.Skip(2).Aggregate(string.Empty, (a, b) => a + b).ToString() : best_pair.Item1 + best_pair.Item2;
                
                _tokens.Add(ntok, _nextId++);
            }
        }

        private Dictionary<(string, string), int> Compute_Pair_Score(List<(string, int)> freqs, Dictionary<string, IEnumerable<string>> word_symbols)
        {
            Dictionary<(string, string), int> pair_freqs = new Dictionary<(string, string), int>();
            Dictionary<(string, string), int> score_freqs = new Dictionary<(string, string), int>(); //Scored freqs.
            Dictionary<string, int> letter_freqs = new Dictionary<string, int> ();
            foreach (var (item, freq) in freqs)
            {
                var split = word_symbols[item];
                if (split.Count() == 1)
                {
                    if (letter_freqs.ContainsKey(split.ElementAt(0)))
                        letter_freqs[split.ElementAt(0)] += 1;
                    else
                        letter_freqs.Add(split.ElementAt(0), 1);
                    continue;
                }
                for (int i = 0; i < split.Count() - 1; i++)
                {
                    var pair = (split.ElementAt(i), split.ElementAt(i + 1));
                    if (letter_freqs.ContainsKey(split.ElementAt(i)))
                        letter_freqs[split.ElementAt(i)] += freq;
                    else
                        letter_freqs.Add(split.ElementAt(i), freq);

                    if(pair_freqs.ContainsKey(pair))
                        pair_freqs[pair] += freq;
                    else
                        pair_freqs.Add(pair, freq);
                }

                if (letter_freqs.ContainsKey(split.Last()))
                    letter_freqs[split.Last()] += freq;
                else
                    letter_freqs.Add(split.Last(), freq);
            }
            foreach(var kv in pair_freqs)
            {
                score_freqs.Add(kv.Key, kv.Value / (letter_freqs[kv.Key.Item1] * letter_freqs[kv.Key.Item2]) );
            }

            return score_freqs;
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
                while (i < split.Count() - 1)
                {
                    if (split.ElementAt(i) == a && split.ElementAt(i + 1) == b)
                    {
                        //Just remove '##' from merging parts.
                        var merge = (b.StartsWith("##")) ? new List<string>() { a, b.Skip(2).Aggregate(string.Empty, (s1, c1) => s1 + c1).ToString()} : new List<string>() { a, b };
                        split = split.Take(i).Union(merge).Union(split.Skip(i + 2));
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