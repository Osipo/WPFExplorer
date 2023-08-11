using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfExplorer.Models.ML.Normalizers
{
    public class NFKCNormalizer : INormalizer
    {
        public string Normalize(string content)
        {
            byte[] nbytes = Encoding.Unicode.GetBytes(content.Normalize(NormalizationForm.FormKC));
            return Encoding.Unicode.GetString(nbytes);
        }
    }
}
