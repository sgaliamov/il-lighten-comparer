using System;
using System.Collections.Generic;

namespace ILLightenComparer.Benchmarks.Benchmark
{
    public sealed class ActorsCollection : IComparable<ActorsCollection>, IComparable
    {
        public Dictionary<int, string> Actors { get; set; }

        public int CompareTo(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return 1;
            }

            if (ReferenceEquals(this, obj))
            {
                return 0;
            }

            return obj is ActorsCollection other
                       ? CompareTo(other)
                       : throw new ArgumentException($"Object must be of type {nameof(ActorsCollection)}");
        }

        public int CompareTo(ActorsCollection other)
        {
            using (var enumeratorX = Actors.GetEnumerator())
            using (var enumeratorY = other.Actors.GetEnumerator())
            {
                while (true)
                {
                    var xDone = !enumeratorX.MoveNext();
                    var yDone = !enumeratorY.MoveNext();

                    if (xDone)
                    {
                        return yDone ? 0 : -1;
                    }

                    if (yDone)
                    {
                        return 1;
                    }

                    // ReSharper disable UseDeconstruction
                    var xCurrent = enumeratorX.Current;
                    var yCurrent = enumeratorY.Current;
                    // ReSharper restore UseDeconstruction

                    var compare = xCurrent.Key.CompareTo(yCurrent.Key);
                    if (compare != 0)
                    {
                        return compare;
                    }

                    compare = string.Compare(xCurrent.Value, yCurrent.Value, StringComparison.Ordinal);
                    if (compare != 0)
                    {
                        return compare;
                    }
                }
            }
        }
    }

    public sealed class MovieSampleObject
    {
        public static IComparer<MovieSampleObject> Comparer { get; } = new MovieSampleObjectRelationalComparer();

        public ActorsCollection Actors { get; set; }
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

                var actorsComparison = x.Actors.CompareTo(y.Actors);
                if (actorsComparison != 0)
                {
                    return actorsComparison;
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
