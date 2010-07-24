﻿using System;
using NUnit.Framework;
using System.Linq;
using Moq;
using Autofac.Core.Activators.Reflection;
using Autofac.Core;
using Autofac.Tests.Scenarios.Dependencies;
using Autofac.Tests.Scenarios.ConstructorSelection;

namespace Autofac.Tests.Core.Activators.Reflection
{
    [TestFixture]
    public class ReflectionActivatorTests
    {
        [Test]
        public void Constructor_DoesNotAcceptNullType()
        {
            Assertions.AssertThrows<ArgumentNullException>(delegate
            {
                new ReflectionActivator(null,
                    new Mock<IConstructorFinder>().Object,
                    new Mock<IConstructorSelector>().Object,
                    Factory.NoParameters,
                    Factory.NoProperties);
            });
        }

        [Test]
        public void Constructor_DoesNotAcceptNullParameters()
        {
            Assertions.AssertThrows<ArgumentNullException>(delegate
            {
                new ReflectionActivator(typeof(object),
                    new Mock<IConstructorFinder>().Object,
                    new Mock<IConstructorSelector>().Object,
                    null,
                    Factory.NoProperties);
            });
        }

        [Test]
        public void Constructor_DoesNotAcceptNullProperties()
        {
            Assertions.AssertThrows<ArgumentNullException>(delegate
            {
                new ReflectionActivator(typeof(object),
                    new Mock<IConstructorFinder>().Object,
                    new Mock<IConstructorSelector>().Object,
                    Factory.NoParameters,
                    null);
            });
        }

        [Test]
        public void Constructor_DoesNotAcceptNullFinder()
        {
            Assertions.AssertThrows<ArgumentNullException>(delegate
            {
                new ReflectionActivator(typeof(object),
                    null,
                    new Mock<IConstructorSelector>().Object,
                    Factory.NoParameters,
                    Factory.NoProperties);
            });
        }

        [Test]
        public void Constructor_DoesNotAcceptNullSelector()
        {
            Assertions.AssertThrows<ArgumentNullException>(delegate
            {
                new ReflectionActivator(typeof(object),
                    new Mock<IConstructorFinder>().Object,
                    null,
                    Factory.NoParameters,
                    Factory.NoProperties);
            });
        }

        [Test]
        public void ActivateInstance_ReturnsInstanceOfTargetType()
        {
            var target = Factory.CreateReflectionActivator(typeof(object));
            var instance = target.ActivateInstance(new Container(), Factory.NoParameters);

            Assert.IsNotNull(instance);
            Assert.IsInstanceOf<object>(instance.GetType());
        }

        [Test]
        public void ActivateInstance_ResolvesConstructorDependencies()
        {
            var o = new object();
            const string s = "s";

            var builder = new ContainerBuilder();
            builder.RegisterInstance(o);
            builder.RegisterInstance(s);
            var container = builder.Build();

            var target = Factory.CreateReflectionActivator(typeof(Dependent));
            var instance = target.ActivateInstance(container, Factory.NoParameters);

            Assert.IsNotNull(instance);
            Assert.IsInstanceOf<Dependent>(instance);

            var dependent = (Dependent)instance;

            Assert.AreSame(o, dependent.TheObject);
            Assert.AreSame(s, dependent.TheString);
       }

        [Test]
        public void ActivateInstance_DependenciesNotAvailable_ThrowsException()
        {
            var target = Factory.CreateReflectionActivator(typeof(DependsByCtor));
            var ex = Assert.Throws<DependencyResolutionException>(
                () => target.ActivateInstance(Factory.EmptyContext, Factory.NoParameters));

            // I.e. the type of the missing dependency.
            Assert.That(ex.Message.Contains("DependsByProp"));
        }

        [Test]
        public void ByDefault_ChoosesConstructorWithMostResolvableParameters()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType(typeof(object));
            var container = builder.Build();

            var target = Factory.CreateReflectionActivator(typeof(MultipleConstructors));
            var instance = target.ActivateInstance(container, Factory.NoParameters);

            Assert.IsNotNull(instance);
            Assert.IsInstanceOf<MultipleConstructors>(instance);
        }

        class AcceptsObjectParameter
        {
            public readonly object P;
            public AcceptsObjectParameter(object p) { P = p; }
        }

        [Test]
        public void ProvidedParameters_OverrideThoseInContext()
        {
            var containedInstance = new object();

            var builder = new ContainerBuilder();
            builder.RegisterInstance(containedInstance);
            var container = builder.Build();

            var parameterInstance = new object();
            var parameters = new Parameter[]{ new NamedParameter("p", parameterInstance)};

            var target = Factory.CreateReflectionActivator(typeof(AcceptsObjectParameter), parameters);

            var instance = (AcceptsObjectParameter)target.ActivateInstance(container, Factory.NoParameters);

            Assert.AreSame(parameterInstance, instance.P);
            Assert.AreNotSame(containedInstance, instance.P);
        }

        [Test]
        public void WhenReferenceTypeParameterSupplied_ItIsProvidedToTheComponent()
        {
            var p = new object();
            var parameters = new Parameter[] { new NamedParameter("p", p) };

            var target = Factory.CreateReflectionActivator(typeof(AcceptsObjectParameter), parameters);

            var instance = target.ActivateInstance(Container.Empty, Factory.NoParameters);

            Assert.IsNotNull(instance);
            Assert.IsInstanceOf<AcceptsObjectParameter>(instance);

            var typedInstance = (AcceptsObjectParameter)instance;

            Assert.AreEqual(p, typedInstance.P);
        }

        [Test]
        public void WhenNullReferenceTypeParameterSupplied_ItIsPassedToTheComponent()
        {
            var parameters = new Parameter[] { new NamedParameter("p", null) };

            var target = Factory.CreateReflectionActivator(typeof(AcceptsObjectParameter), parameters);

            var instance = target.ActivateInstance(Container.Empty, Factory.NoParameters);

            Assert.IsNotNull(instance);
            Assert.IsInstanceOf<AcceptsObjectParameter>(instance);

            var typedInstance = (AcceptsObjectParameter)instance;

            Assert.IsNull(typedInstance.P);
        }

        class AcceptsIntParameter
        {
            public readonly int I;
            public AcceptsIntParameter(int i) { I = i; }
        }

        [Test]
        public void WhenValueTypeParameterSupplied_ItIsPassedToTheComponent()
        {
            const int i = 42;
            var parameters = new Parameter[] { new NamedParameter("i", i) };

            var target = Factory.CreateReflectionActivator(typeof(AcceptsIntParameter), parameters);

            var instance = target.ActivateInstance(new Container(), Factory.NoParameters);

            Assert.IsNotNull(instance);
            Assert.IsInstanceOf<AcceptsIntParameter>(instance);

            var typedInstance = (AcceptsIntParameter)instance;

            Assert.AreEqual(i, typedInstance.I);
        }

        [Test]
        public void WhenValueTypeParameterIsSuppliedWithNull_TheDefaultForTheValueTypeIsSet()
        {
            var parameters = new Parameter[] { new NamedParameter("i", null) };

            var target = Factory.CreateReflectionActivator(typeof(AcceptsIntParameter), parameters);

            var instance = target.ActivateInstance(new Container(), Factory.NoParameters);

            Assert.IsNotNull(instance);
            Assert.IsInstanceOf<AcceptsIntParameter>(instance);

            var typedInstance = (AcceptsIntParameter)instance;

            Assert.AreEqual(0, typedInstance.I);
        }

        class ThreeConstructors
        {
            public readonly int CalledConstructorParameterCount;
            // ReSharper disable UnusedMember.Local, UnusedParameter.Local
            public ThreeConstructors() { CalledConstructorParameterCount = 0; }
            public ThreeConstructors(int i, string s) { CalledConstructorParameterCount = 2; }
            public ThreeConstructors(int i) { CalledConstructorParameterCount = 1; }
            // ReSharper restore UnusedMember.Local, UnusedParameter.Local
        }

        [Test]
        public void ByDefault_ChoosesMostParameterisedConstructor()
        {
            var parameters = new Parameter[] {
                new NamedParameter("i", 1),
                new NamedParameter("s", "str")
            };

            var target = Factory.CreateReflectionActivator(typeof(ThreeConstructors), parameters);

            var instance = target.ActivateInstance(new Container(), Factory.NoParameters);

            Assert.IsNotNull(instance);
            Assert.IsInstanceOf<ThreeConstructors>(instance);

            var typedInstance = (ThreeConstructors)instance;

            Assert.AreEqual(2, typedInstance.CalledConstructorParameterCount);
        }

        class NoPublicConstructor
        {
            // ReSharper disable EmptyConstructor
            internal NoPublicConstructor() { }
            // ReSharper restore EmptyConstructor
        }

        [Test]
        [ExpectedException(typeof(DependencyResolutionException))]
        public void NonPublicConstructorsIgnored()
        {
            var target = Factory.CreateReflectionActivator(typeof(NoPublicConstructor));
            target.ActivateInstance(new Container(), Factory.NoParameters);
        }

        public class WithGenericCtor<T>
        {
            // ReSharper disable UnusedParameter.Local
            public WithGenericCtor(T t)
            // ReSharper restore UnusedParameter.Local
            {
            }
        }

        [Test]
        public void CanResolveConstructorsWithGenericParameters()
        {
            var activator = Factory.CreateReflectionActivator(typeof(WithGenericCtor<string>));
            var parameters = new Parameter[] { new NamedParameter("t", "Hello") };
            var instance = activator.ActivateInstance(new Container(), parameters);
            Assert.IsInstanceOf<WithGenericCtor<string>>(instance);
        }

        class PrivateSetProperty
        {
            // ReSharper disable UnusedMember.Local
            public int GetProperty { private set; get; }
            public int P { get; set; }
            // ReSharper restore UnusedMember.Local
        }

        [Test]
        public void PropertiesWithPrivateSetters_AreIgnored()
        {
            var setters = new Parameter[] { new NamedPropertyParameter("P", 1) };
            var activator = Factory.CreateReflectionActivator(typeof(PrivateSetProperty), Factory.NoParameters, setters);
            var instance = activator.ActivateInstance(new Container(), Factory.NoParameters);
            Assert.IsInstanceOf<PrivateSetProperty>(instance);
        }

        // ReSharper disable UnusedAutoPropertyAccessor.Local
        class R { public int P1 { get; set; } public int P2 { get; set; } }
        // ReSharper restore UnusedAutoPropertyAccessor.Local

        [Test]
        public void SetsMultipleConfiguredProperties()
        {
            const int p1 = 1;
            const int p2 = 2;
            var properties = new [] {
                new NamedPropertyParameter("P1", p1),
                new NamedPropertyParameter("P2", p2)
            };
            var target = Factory.CreateReflectionActivator(typeof(R), Enumerable.Empty<Parameter>(), properties);
            var instance = (R)target.ActivateInstance(Container.Empty, Enumerable.Empty<Parameter>());
            Assert.AreEqual(1, instance.P1);
            Assert.AreEqual(2, instance.P2);
        }

    }
}
