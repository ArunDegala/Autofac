﻿//using System;
//using System.Collections.Generic;
//using Autofac.Builder;
//using Autofac.Component;
//using Autofac.Component.Activation;
//using Autofac.Registrars;
//using Autofac.Registrars.Generic;
//using NUnit.Framework;
//using System.Linq;

//namespace Autofac.Tests.Registrars.Generic
//{
//    [TestFixture]
//    public class GenericRegistrationHandlerFixture
//    {
//        interface I<T> { }

//        class A1<T> : DisposeTracker, I<T> { }

//        [Test]
//        public void RegistrationProvided()
//        {
//            var c = new Container();
//            c.ComponentRegistry.AddRegistrationSource(new GenericRegistrationHandler(
//                new Service[] { new TypedService(typeof(I<>)) },
//                typeof(A1<>),
//                new DeferredRegistrationParameters(
//                    InstanceOwnership.Container,
//                    InstanceSharing.Singleton,
//                    new EventHandler<PreparingEventArgs>[] { },
//                    new EventHandler<ActivatingEventArgs>[] { },
//                    new EventHandler<ActivatedEventArgs>[] { },
//                    (d, a, s, o) => new Registration(d, a, s, o)),
//                new MostParametersConstructorSelector(),
//                Enumerable.Empty<Parameter>(),
//                Enumerable.Empty<NamedPropertyParameter>()));

//            var x = c.Resolve<I<int>>();
//            var x2 = c.Resolve<I<int>>();

//            Assert.IsNotNull(x);
//            Assert.AreSame(x, x2);
//            Assert.AreEqual(typeof(A1<>), x.GetType().GetGenericTypeDefinition());

//            c.Dispose();

//            Assert.IsTrue(((DisposeTracker)x).IsDisposed);
//        }
		
//        [Test]
//        public void GenericRegistrationsInSubcontextOverrideRootContext()
//        {
//            var builder = new ContainerBuilder();
//            builder.RegisterGeneric(typeof(List<>)).As(typeof(ICollection<>)).FactoryScoped();
//            var container = builder.Build();
//            var inner = container.BeginLifetimeScope();
//            var innerBuilder = new ContainerBuilder();
//            innerBuilder.RegisterGeneric(typeof(LinkedList<>)).As(typeof(ICollection<>)).FactoryScoped();
//            innerBuilder.Build(inner);
			
//            var list = inner.Resolve<ICollection<int>>();
//            Assert.IsInstanceOfType(typeof(LinkedList<int>), list);
//        }
		
//        [Test]
//        public void SingletonGenericComponentsResolvedInSubcontextStickToParent()
//        {
//            var builder = new ContainerBuilder();
//            builder.RegisterGeneric(typeof(List<>)).As(typeof(ICollection<>));
//            var container = builder.Build();
//            var inner = container.BeginLifetimeScope();
			
//            var innerList = inner.Resolve<ICollection<int>>();
//            var outerList = container.Resolve<ICollection<int>>();
//            Assert.AreSame(innerList, outerList);
//        }
		
//        [Test]
//        public void GenericCircularityAvoidedWithUsingContstructor()
//        {
//            var builder = new ContainerBuilder();
//            builder.RegisterGeneric(typeof(List<>))
//                .As(typeof(IEnumerable<>))
//                .UsingConstructor(new Type[] {});
//            var container = builder.Build();
//            var list = container.Resolve<IEnumerable<int>>();
//        }
//    }
//}
