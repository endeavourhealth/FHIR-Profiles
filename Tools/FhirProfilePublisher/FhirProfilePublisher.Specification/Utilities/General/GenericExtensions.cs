using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FhirProfilePublisher.Specification
{
    public static class GenericExtensions
    {
        public static R WhenNotNull<T, R>(this T source, Func<T, R> selector)
        {
            if (source == null)
                return default(R);

            return selector(source);
        }

        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> seenKeys = new HashSet<TKey>();

            foreach (TSource element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }

        public static IEnumerable<T> AllExceptLast<T>(this IEnumerable<T> enumerable)
        {
            return enumerable.Take(enumerable.Count() - 1);
        }

        public static IEnumerable<T> Intersperse<T>(this IEnumerable<T> items, T separator)
        {
            bool first = true;

            foreach (T item in items)
            {
                if (first) 
                    first = false;
                else
                    yield return separator;

                yield return item;
            }
        }
    }
}
