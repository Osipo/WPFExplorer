using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfExplorer.Models.ML.Normalizers
{
    public class SimpleBERTNormalizer : INormalizer
    {
        
        public string Normalize(string content)
        {
            //"Héllò hôw are ü?"

            //1. Apply NFD Normalization.
            byte[] nbytes = Encoding.Unicode.GetBytes(content.Normalize(NormalizationForm.FormD));
            string normalized =  Encoding.Unicode.GetString(nbytes);

            //2. To Lower case
            normalized = normalized.ToLower();

            //3. Strip accent.
            var stringBuilder = new StringBuilder(capacity: normalized.Length);

            for (int i = 0; i < normalized.Length; i++)
            {
                char c = normalized[i];
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder
                .ToString()
                .Normalize(NormalizationForm.FormC);
        }
    }
}
