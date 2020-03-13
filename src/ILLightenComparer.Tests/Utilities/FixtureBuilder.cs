using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using AutoFixture;
using AutoFixture.Kernel;
using Force.DeepCloner;
using Xunit;

namespace ILLightenComparer.Tests.Utilities
{
    public static class FixtureBuilder
    {
        private static readonly Lazy<Fixture> Fixture = new Lazy<Fixture>(
            () => {
                var f = new Fixture { RepeatCount = Constants.SmallCount };

                f.Customize(new DomainCustomization());

                return f;
            },
            LazyThreadSafetyMode.ExecutionAndPublication);

        private static readonly Lazy<Fixture> SimpleFixture = new Lazy<Fixture>(
            () => {
                var f = new Fixture { RepeatCount = Constants.SmallCount };

                f.Customize(new SupportMutableValueTypesCustomization());
                f.Behaviors.Add(new OmitOnRecursionBehavior());

                return f;
            },
            LazyThreadSafetyMode.ExecutionAndPublication);

        public static Fixture GetInstance() => Fixture.Value;

        public static Fixture GetSimpleInstance() => SimpleFixture.Value;

        public static IEnumerable<T> CreateMutants<T>(this Fixture _, T prototype)
        {
            if (typeof(T).IsValueType) {
                throw new ArgumentException("T should be a class.", nameof(T));
            }

            return Process();

            IEnumerable<T> Process()
            {
                var clone = prototype.DeepClone();
                foreach (var member in new ObjectWalker(new Member(clone))) {
                    if (member.Parent?.GetType().IsValueType ?? false) {
                        continue;
                    }

                    if (!member.ValueType.IsPrimitive() && member.Value != null) {
                        continue;
                    }

                    var setValue = GetSetValueAction(member);

                    setValue(
                        member.Parent,
                        GetNewValue(member.ValueType, member.Value));

                    yield return clone.DeepClone();

                    setValue(member.Parent, member.Value);
                }
            }
        }

        public static object Create(this Fixture fixture, Type type)
        {
            var context = fixture.GetOrAddProperty(nameof(SpecimenContext), () => new SpecimenContext(fixture));

            return context.Resolve(type);
        }

        private static Action<object, object> GetSetValueAction(Member member) => member.MemberInfo switch
        {
            FieldInfo fieldInfo => fieldInfo.SetValue,
            PropertyInfo propertyInfo => propertyInfo.SetValue,
            _ => throw new InvalidOperationException(),
        };

        private static object GetNewValue(Type type, object oldValue)
        {
            var times = 5;
            while (true) {
                var newValue = SimpleFixture.Value.Create(type);
                if (!newValue.Equals(oldValue)) {
                    return newValue;
                }

                Assert.NotEqual(0, --times);
            }
        }
    }
}
