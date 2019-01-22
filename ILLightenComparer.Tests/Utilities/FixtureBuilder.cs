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
                var f = new Fixture { RepeatCount = Constants.SmallCount };

                f.Customize(new DomainCustomization());
                f.Behaviors.Add(new OmitOnRecursionBehavior());

                return f;
            },
            LazyThreadSafetyMode.ExecutionAndPublication);

        public static Fixture GetInstance()
        {
            return Fixture.Value;
        }

        public static IEnumerable<T> CreateMutants<T>(this Fixture fixture, T prototype)
        {
            if (typeof(T).IsValueType)
            {
                throw new ArgumentException("T should be a class.", nameof(T));
            }

            var clone = prototype.DeepClone();
            foreach (var member in new ObjectWalker(new Member(clone)))
            {
                if (member.Parent?.GetType().IsValueType ?? false)
                {
                    continue;
                }

                if (!member.ValueType.IsPrimitive() && member.Value != null)
                {
                    continue;
                }

                var setValue = GetSetValueAction(member);

                setValue(
                    member.Parent,
                    GetNewValue(fixture, member.ValueType, member.Value));

                yield return clone.DeepClone();

                setValue(member.Parent, member.Value);
            }
        }

        public static object Create(this Fixture fixture, Type type)
        {
            var context = fixture.GetOrAddProperty(nameof(SpecimenContext), () => new SpecimenContext(fixture));

            return context.Resolve(type);
        }

        private static Action<object, object> GetSetValueAction(Member member)
        {
            switch (member.MemberInfo)
            {
                case FieldInfo fieldInfo: return fieldInfo.SetValue;
                case PropertyInfo propertyInfo: return propertyInfo.SetValue;
                default: throw new InvalidOperationException();
            }
        }

        private static object GetNewValue(Fixture fixture, Type type, object oldValue)
        {
            while (true)
            {
                var newValue = fixture.Create(type);
                if (!newValue.Equals(oldValue))
                {
                    return newValue;
                }
            }
        }
    }
}
