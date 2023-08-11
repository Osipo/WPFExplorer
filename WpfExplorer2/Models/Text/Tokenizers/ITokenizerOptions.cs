using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfExplorer.Models.ML.Normalizers;

namespace WpfExplorer.Models.ML
{
    public interface ITokenizerOptions
    {
        IList<INormalizer> Normalizers { get; }
        ITokenizer Tokenizer { get; set; }
    }
}
