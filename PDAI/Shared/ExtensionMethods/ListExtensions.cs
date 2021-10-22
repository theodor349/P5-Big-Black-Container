using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.ExtensionMethods
{
    public static class ListExtensions
    {
        public static List<List<T>> ChunkBy<T>(this List<T> source, int chunkSize)
        {
            return source
                .Select((s, i) => new { s, i })
                .GroupBy(x => x.i % chunkSize)
                .Select(g => g.Select(x => x.s).ToList())
                .ToList();
        }
    }
}
