using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WpfExplorer.Models.ML.Normalizers
{
    public class WhitespaceNormalizer : INormalizer
    {
        public string Normalize(string content)
        {
            RegexOptions ecmaOptions = RegexOptions.ECMAScript | RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled;
            Regex regex = new Regex("[\\s]{2,}", ecmaOptions); //except 0x85, '...' and p{Z} Unicode separator category.
            return regex.Replace(content, " ");
        }
    }
}
