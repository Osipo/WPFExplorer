using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfExplorer.Models.ML
{
    public interface ITokenizer
    {
        IEnumerable<int> EncodedTokens(string content);
        IEnumerable<int> EncodedTokens(IEnumerable<string> words);
        IEnumerable<string> DecodedTokens(IEnumerable<int> encoded);
    }
}
