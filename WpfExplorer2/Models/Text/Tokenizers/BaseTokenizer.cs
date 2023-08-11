using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfExplorer.Models.ML.Normalizers;

namespace WpfExplorer.Models.ML
{
    public class BaseTokenizer : ITokenizerOptions
    {
        private IList<INormalizer> _normalizers;
        private ITokenizer _tokenizer;


        public BaseTokenizer() {
            _normalizers = new List<INormalizer>();
            _tokenizer = new WordPieceEncoder(50);
            _normalizers.Add(new SimpleBERTNormalizer());
            _normalizers.Add(new WhitespaceNormalizer());
        }

        public BaseTokenizer(ITokenizer tokenizer)
        {
            _normalizers = new List<INormalizer>();
            _tokenizer = tokenizer;
        }

        public IList<INormalizer> Normalizers { get { return _normalizers; } }

        public ITokenizer Tokenizer { get { return _tokenizer; } set { _tokenizer = value; } }

        public IEnumerable<int> Encode(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                return null;

            for(int i = 0; i <  _normalizers.Count; i++)
            {
                content = _normalizers[i].Normalize(content);
                if (string.IsNullOrWhiteSpace(content))
                    return null;
            }
            return Tokenizer.EncodedTokens(content);
        }

        public IEnumerable<string> Decode(IEnumerable<int> tokens)
        {
            return Tokenizer.DecodedTokens(tokens);
        }
    }
}
