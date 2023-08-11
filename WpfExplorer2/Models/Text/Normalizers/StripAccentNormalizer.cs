using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfExplorer.Models.ML.Normalizers
{
    public class StripAccentNormalizer : INormalizer
    {
        public string Normalize(string content)
        {
            //Apply Normalization Form D (nfd) if content is not normalized by NFD.
            var normalized = content;
            if(!normalized.IsNormalized(NormalizationForm.FormD))
                normalized = content.Normalize(NormalizationForm.FormD);

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
