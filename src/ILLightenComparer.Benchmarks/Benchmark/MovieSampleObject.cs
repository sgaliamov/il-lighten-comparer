using System;
using System.Collections.Generic;

namespace ILLightenComparer.Benchmarks.Benchmark
{
    public sealed class MovieSampleObject
    {
        public static IComparer<MovieSampleObject> Comparer { get; } = new MovieSampleObjectRelationalComparer();

        public Dictionary<int, string> Actors { get; set; }
        public string Genre { get; set; }
        public int Id { get; set; }
        public decimal Price { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string Title { get; set; }

        private sealed class MovieSampleObjectRelationalComparer : IComparer<MovieSampleObject>
        {
            public int Compare(MovieSampleObject x, MovieSampleObject y)
            {
                if (ReferenceEquals(x, y))
                {
                    return 0;
                }

                if (ReferenceEquals(null, y))
                {
                    return 1;
                }

                if (ReferenceEquals(null, x))
                {
                    return -1;
                }

                using (var enumeratorX = x.Actors.GetEnumerator())
                using (var enumeratorY = y.Actors.GetEnumerator())
                {
                    while (true)
                    {
                        var xDone = !enumeratorX.MoveNext();
                        var yDone = !enumeratorY.MoveNext();

                        if (xDone)
                        {
                            if (yDone)
                            {
                                break;
                            }

                            return -1;
                        }

                        if (yDone)
                        {
                            return 1;
                        }

                        var (xKey, xValue) = enumeratorX.Current;
                        var (yKey, yValue) = enumeratorY.Current;

                        var compare = xKey.CompareTo(yKey);
                        if (compare != 0)
                        {
                            return compare;
                        }

                        compare = string.Compare(xValue, yValue, StringComparison.Ordinal);
                        if (compare != 0)
                        {
                            return compare;
                        }
                    }
                }

                var genreComparison = string.Compare(x.Genre, y.Genre, StringComparison.Ordinal);
                if (genreComparison != 0)
                {
                    return genreComparison;
                }

                var idComparison = x.Id.CompareTo(y.Id);
                if (idComparison != 0)
                {
                    return idComparison;
                }

                var priceComparison = x.Price.CompareTo(y.Price);
                if (priceComparison != 0)
                {
                    return priceComparison;
                }

                var releaseDateComparison = x.ReleaseDate.CompareTo(y.ReleaseDate);
                if (releaseDateComparison != 0)
                {
                    return releaseDateComparison;
                }

                return string.Compare(x.Title, y.Title, StringComparison.Ordinal);
            }
        }
    }
}
