using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using AutoFixture;
using AutoFixture.Kernel;
using Force.DeepCloner;

namespace ILLightenComparer.Tests.Utilities
{
    public static class FixtureBuilder
    {
        private static readonly Lazy<Fixture> Fixture = new Lazy<Fixture>(
            () =>
            {
                var f = new Fixture { RepeatCount = 2 };

                f.Customize(new DomainCustomization());
                f.Behaviors.Add(new OmitOnRecursionBehavior());

                return f;
            },
            LazyThreadSafetyMode.ExecutionAndPublication);

        public static Fixture GetInstance() => Fixture.Value;

        public static IEnumerable<T> CreateMutants<T>(T prototype)
        {
            var context = new SpecimenContext(Fixture.Value);

            var clone = prototype.DeepClone();
            foreach (var member in new ObjectWalker(new Member(clone)))
            {
                if (!member.ValueType.IsPrimitive() && member.Value != null)
                {
                    continue;
                }

                switch (member.MemberInfo)
                {
                    case FieldInfo fieldInfo:
                        fieldInfo.SetValue(
                            member.Parent,
                            GetNewValue(context, fieldInfo.FieldType, member.Value));
                        yield return clone.DeepClone();
                        fieldInfo.SetValue(member.Parent, member.Value);
                        break;

                    case PropertyInfo propertyInfo:
                        propertyInfo.SetValue(
                            member.Parent,
                            GetNewValue(context, propertyInfo.PropertyType, member.Value));
                        yield return clone.DeepClone();
                        propertyInfo.SetValue(member.Parent, member.Value);
                        break;
                }
            }
        }

        private static object GetNewValue(ISpecimenContext context, Type type, object oldValue)
        {
            while (true)
            {
                var newValue = context.Resolve(type);
                if (!newValue.Equals(oldValue))
                {
                    return newValue;
                }
            }
        }
    }
}
