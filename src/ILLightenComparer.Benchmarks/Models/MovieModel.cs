using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace ILLightenComparer.Benchmarks.Models
{
    public sealed class MovieModel
    {
        public ActorsCollection Actors { get; set; }
        public string Genre { get; set; }
        public int Id { get; set; }
        public decimal Price { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string Title { get; set; }
    }

    [SuppressMessage("Design", "CA1036:Override methods on comparable types", Justification = "<Pending>")]
    public sealed class ActorsCollection : IComparable<ActorsCollection>, IComparable
    {
        public Dictionary<int, string> Actors { get; set; }

        public int CompareTo(object obj) =>
            obj is ActorsCollection other
                ? CompareTo(other)
                : throw new ArgumentException($"Object must be of type {nameof(ActorsCollection)}");

        public int CompareTo(ActorsCollection other)
        {
            if (other is null) {
                return 1;
            }

            if (ReferenceEquals(this, other)) {
                return 0;
            }

            if (Actors == null) {
                return other.Actors == null ? 0 : -1;
            }

            if (other.Actors == null) {
                return 1;
            }

            using var enumeratorX = Actors.GetEnumerator();
            using var enumeratorY = other.Actors.GetEnumerator();

            while (true) {
                var xDone = !enumeratorX.MoveNext();
                var yDone = !enumeratorY.MoveNext();

                if (xDone) {
                    return yDone ? 0 : -1;
                }

                if (yDone) {
                    return 1;
                }

                var (keyX, valueX) = enumeratorX.Current;
                var (keyY, valueY) = enumeratorY.Current;

                var compare = keyX.CompareTo(keyY);
                if (compare != 0) {
                    return compare;
                }

                compare = string.CompareOrdinal(valueX, valueY);
                if (compare != 0) {
                    return compare;
                }
            }
        }
    }

    internal sealed class MovieModelComparer : IComparer<MovieModel>
    {
        public static IComparer<MovieModel> Instance { get; } = new MovieModelComparer();

        public int Compare(MovieModel x, MovieModel y)
        {
            if (ReferenceEquals(x, y)) {
                return 0;
            }

            if (y is null) {
                return 1;
            }

            if (x is null) {
                return -1;
            }

            if (x.Actors != null) {
                var actorsComparison = x.Actors.CompareTo(y.Actors);
                if (actorsComparison != 0) {
                    return actorsComparison;
                }
            } else if (y.Actors != null) {
                return -1;
            }

            var genreComparison = string.CompareOrdinal(x.Genre, y.Genre);
            if (genreComparison != 0) {
                return genreComparison;
            }

            var idComparison = x.Id.CompareTo(y.Id);
            if (idComparison != 0) {
                return idComparison;
            }

            var priceComparison = x.Price.CompareTo(y.Price);
            if (priceComparison != 0) {
                return priceComparison;
            }

            var releaseDateComparison = x.ReleaseDate.CompareTo(y.ReleaseDate);
            if (releaseDateComparison != 0) {
                return releaseDateComparison;
            }

            return string.CompareOrdinal(x.Title, y.Title);
        }
    }
}
