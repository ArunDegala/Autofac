﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Autofac.Core;

namespace Autofac.Tests
{
    [TestFixture]
    public class RegistrationExtensionsTests
    {
        // ReSharper disable InconsistentNaming

        interface IMyService { }

        sealed class MyComponent : IMyService { }

        [Test]
        public void RegistrationsMadeInConfigureExpressionAreAddedToContainer()
        {
            var ls = new Container()
                .BeginLifetimeScope(b => b.RegisterType<MyComponent>().As<IMyService>());

            var component = ls.Resolve<IMyService>();
            Assert.IsTrue(component is MyComponent);
        }
    }
}
