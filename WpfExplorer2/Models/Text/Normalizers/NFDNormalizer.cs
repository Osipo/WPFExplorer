using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfExplorer.Models.ML.Normalizers
{
    /// <summary>
    /// Normalize Unicode in Normalization Form D (NFD)
    /// </summary>
    public class NFDNormalizer : INormalizer
    {
        public string Normalize(string content)
        {
            byte[] nbytes = Encoding.Unicode.GetBytes(content.Normalize(NormalizationForm.FormD));
            return Encoding.Unicode.GetString(nbytes);
        }
    }
}
