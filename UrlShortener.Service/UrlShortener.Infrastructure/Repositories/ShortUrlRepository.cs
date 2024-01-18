using System.Text;
using UrlShortener.Domain.Interfaces;

namespace UrlShortener.Infrastructure.Repositories
{
    public class ShortUrlRepository : IShortUrlRepository
    {
        public static readonly string Alphabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        private readonly IDictionary<char, int> AlphabetIndex;
        public static readonly int Base = Alphabet.Length;
        
        public ShortUrlRepository()
        {
            AlphabetIndex = Alphabet
                .Select((c, i) => new { Index = i, Char = c })
                .ToDictionary(c => c.Char, c => c.Index);
        }

        public string GenerateShortString(int seed)
        {
            if (seed < Base)
            {
                return Alphabet[0].ToString();
            }
            
            var str = new StringBuilder();
            var i = seed;

            while (i > 0)
            {
                str.Insert(0, Alphabet[i % Base]);
                i /= Base;
            }

            return str.ToString();
        }

        public int RestoreSeedFromString(string str)
        {
            return str.Aggregate(0, (current, c) => current * Base + AlphabetIndex[c]);
        }
    }
}
