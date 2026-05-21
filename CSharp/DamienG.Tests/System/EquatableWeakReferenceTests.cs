using System;
using DamienG.System;
using Xunit;

namespace DamienG.Tests.System
{
    public class EquatableWeakReferenceTests
    {
        [Fact]
        public void ConstructorThrowsForNullTarget()
        {
            Assert.Throws<ArgumentNullException>(() => new EquatableWeakReference<string>(null));
        }

        [Fact]
        public void TargetReturnsOriginalObjectWhileAlive()
        {
            var obj = new object();
            var weakRef = new EquatableWeakReference<object>(obj);
            Assert.Same(obj, weakRef.Target);
            GC.KeepAlive(obj);
        }

        [Fact]
        public void IsAliveTrueWhileReferenced()
        {
            var obj = new object();
            var weakRef = new EquatableWeakReference<object>(obj);
            Assert.True(weakRef.IsAlive);
            GC.KeepAlive(obj);
        }

        [Fact]
        public void EqualsReturnsTrueForSameTarget()
        {
            var obj = new object();
            var a = new EquatableWeakReference<object>(obj);
            var b = new EquatableWeakReference<object>(obj);
            Assert.True(a.Equals(b));
            Assert.True(a.Equals((object)b));
            GC.KeepAlive(obj);
        }

        [Fact]
        public void EqualsReturnsFalseForDifferentTargets()
        {
            var x = new object();
            var y = new object();
            var a = new EquatableWeakReference<object>(x);
            var b = new EquatableWeakReference<object>(y);
            Assert.False(a.Equals(b));
            GC.KeepAlive(x);
            GC.KeepAlive(y);
        }

        [Fact]
        public void EqualsReturnsFalseForNull()
        {
            var obj = new object();
            var a = new EquatableWeakReference<object>(obj);
            Assert.False(a.Equals(null));
            Assert.False(a.Equals((object)null));
            GC.KeepAlive(obj);
        }

        [Fact]
        public void EqualsReturnsFalseForDifferentType()
        {
            var obj = new object();
            var a = new EquatableWeakReference<object>(obj);
            Assert.False(a.Equals("not a weak reference"));
            GC.KeepAlive(obj);
        }

        [Fact]
        public void GetHashCodeMatchesTargetHashCodeAtConstruction()
        {
            var s = "abc";
            var weakRef = new EquatableWeakReference<string>(s);
            Assert.Equal(s.GetHashCode(), weakRef.GetHashCode());
        }

        [Fact]
        public void DisposeReleasesHandle()
        {
            var obj = new object();
            var weakRef = new EquatableWeakReference<object>(obj);
            weakRef.Dispose();
            // Calling Dispose again must not throw.
            weakRef.Dispose();
            GC.KeepAlive(obj);
        }

        [Fact]
        public void IsAliveFalseAfterTargetCollected()
        {
            var weakRef = CreateOrphanedWeakReference();
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            Assert.False(weakRef.IsAlive);
            Assert.Null(weakRef.Target);
        }

        private static EquatableWeakReference<object> CreateOrphanedWeakReference()
        {
            return new EquatableWeakReference<object>(new object());
        }
    }
}
