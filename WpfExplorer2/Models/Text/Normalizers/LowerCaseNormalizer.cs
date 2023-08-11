using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfExplorer.Models.ML.Normalizers
{
    public class LowerCaseNormalizer : INormalizer
    {
        public string Normalize(string content)
        {
            return content.ToLower();
        }
    }
}
