using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Emit;
using System.Text;

namespace ILLightenComparer.Emit.Shared
{
    internal sealed partial class ILEmitter
    {
        private readonly StringBuilder _debugger = new StringBuilder();
        private readonly List<Label> _debugLabels = new List<Label>();
        private readonly string _name;

        public ILEmitter(string name, ILGenerator il) : this(il)
        {
            _name = name;
        }

        partial void DebugLine(string message)
        {
            _debugger.AppendLine(message);
        }

        partial void DebugMarkLabel(Label label)
        {
            DebugLine($"\tLabel_{_debugLabels.IndexOf(label)}:");
        }

        partial void DebugEmitLabel(OpCode opCode, Label label)
        {
            DebugLine($"\t\t{opCode} Label_{_debugLabels.IndexOf(label)}");
        }

        partial void AddDebugLabel(Label label)
        {
            _debugLabels.Add(label);
        }

        partial void DebugOutput()
        {
            Debug.WriteLine(_name);

            var locals = _localBuckets
                         .Values.SelectMany(x => x.Values)
                         .OrderBy(x => x.LocalIndex)
                         .ToArray();

            if (locals.Length != 0)
            {
                Debug.WriteLine("\t.locals init (");
                foreach (var item in locals)
                {
                    Debug.WriteLine($"\t\t[{item.LocalIndex}] {item.LocalType}");
                }

                Debug.WriteLine("\t)");
            }

            Debug.WriteLine(_debugger.ToString().TrimEnd());
        }
    }
}
