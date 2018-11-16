using System;
using System.Collections;

namespace ILLightenComparer.Emit
{
    internal sealed class ComparerBuilder
    {
        private readonly Context _context;

        public ComparerBuilder(Context context) => _context = context;

        public IComparer Build(Type objectType)
        {
            var typeBuilder = _context.DefineType($"{objectType.FullName}.Comparer", typeof(IComparer));

            return null;
        }
    }
}
