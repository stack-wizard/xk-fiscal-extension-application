using System.Collections.Generic;
using System.Text;

namespace Mikos.XK.Fiscal.Util
{
    public class CyrillicsUtil
    {
        private static readonly Dictionary<char, char> LatinToMacedonianCyrillic = new Dictionary<char, char>
        {
            { 'a', 'а' }, { 'b', 'б' }, { 'c', 'ц' }, { 'd', 'д' }, { 'e', 'е' },
            { 'f', 'ф' }, { 'g', 'г' }, { 'h', 'х' }, { 'i', 'и' }, { 'j', 'ј' },
            { 'k', 'к' }, { 'l', 'л' }, { 'm', 'м' }, { 'n', 'н' }, { 'o', 'о' },
            { 'p', 'п' }, { 'q', 'ќ' }, { 'r', 'р' }, { 's', 'с' }, { 't', 'т' },
            { 'u', 'у' }, { 'v', 'в' }, { 'w', 'њ' }, { 'x', 'џ' }, { 'y', 'ѕ' },
            { 'z', 'з' }
        };

        public static string TranslateToMacedonianCyrillic(string input)
        {
            var translated = new StringBuilder();

            foreach (char c in input)
            {
                if (LatinToMacedonianCyrillic.TryGetValue(char.ToLower(c), out char cyrillicChar))
                {
                    // Preserve case
                    translated.Append(char.IsUpper(c) ? char.ToUpper(cyrillicChar) : cyrillicChar);
                }
                else
                {
                    translated.Append(c); // Keep original character if no mapping found
                }
            }

            return translated.ToString();
        }
    }
}
