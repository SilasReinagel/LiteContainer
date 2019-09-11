using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LiteContainer.Tests
{
    [TestClass]
    public class LiteContainerTests
    {
        [TestMethod]
        public void LiteContainer_ResolveUnknownConcreteType_ThrowsKeyNotFoundException()
        {
            var c = new LiteContainer();

            AssertThrowsExceptionOfType<KeyNotFoundException>(() => c.Resolve<Number>());
        }
        
        [TestMethod]
        public void LiteContainer_ResolveUnknownInterfaceType_ThrowsKeyNotFoundException()
        {
            var c = new LiteContainer();

            AssertThrowsExceptionOfType<KeyNotFoundException>(() => c.Resolve<Person>());
        }
        
        [TestMethod]
        public void LiteContainer_RegisterConcreteTypeTwice_ThrowsInvalidOperationException()
        {
            var c = new LiteContainer();
            c.Register(() => new Number(2));
            
            AssertThrowsExceptionOfType<InvalidOperationException>(() => c.Register(() => new Number(3)));
        }
        
        [TestMethod]
        public void LiteContainer_ResolveConcreteRegisteredType_TypeIsCorrect()
        {
            var c = new LiteContainer();
            c.Register(() => new Number(2));
            
            var instance = c.Resolve<Number>();
            
            Assert.AreEqual(2, instance.Value);
        }
        
        [TestMethod]
        public void LiteContainer_ResolveTypeAsInterface_CanResolveType()
        {
            var c = new LiteContainer();
            c.Register<Person>(() => new Student("Jim"));
            
            var instance = c.Resolve<Person>();
            
            Assert.AreEqual("Jim", instance.Name);
        }

        [TestMethod]
        public void LiteContainer_RegisterSingleton_ReturnsSameReference()
        {
            var c = new LiteContainer();
            c.RegisterSingleton(() => new Number(3));
            
            var instance1 = c.Resolve<Number>();
            var instance2 = c.Resolve<Number>();
            
            Assert.IsTrue(ReferenceEquals(instance1, instance2));
        }

        private void AssertThrowsExceptionOfType<T>(Action performAction) where T : Exception
        {
            try
            {
                performAction();
            }
            catch (Exception ex)
            {
                if (ex.GetType() != typeof(T))
                    Assert.Fail($"Expected exception of type {typeof(T)} to be thrown, but {ex.GetType()} was thrown.");
                return;
            }
            
            Assert.Fail("Expected exception was not thrown.");
        }
    }

    public class Number
    {
        public int Value { get; }
        
        public Number(int data) => Value = data;
    }

    public interface Person
    {
        string Name { get; }
    }

    public sealed class Student : Person
    {
        public string Name { get; }

        public Student(string name) => Name = name;
    }
}
