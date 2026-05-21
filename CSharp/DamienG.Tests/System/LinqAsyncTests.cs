using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DamienG.System;
using Xunit;

namespace DamienG.Tests.System
{
    public class LinqAsyncTests
    {
        private static IQueryable<int> Source => new[] { 1, 2, 3, 4, 5 }.AsQueryable();

        [Fact]
        public async Task FirstAsyncReturnsFirstElement()
        {
            Assert.Equal(1, await Source.FirstAsync());
        }

        [Fact]
        public async Task FirstAsyncWithPredicateReturnsFirstMatch()
        {
            Assert.Equal(3, await Source.FirstAsync(x => x > 2));
        }

        [Fact]
        public async Task FirstOrDefaultAsyncReturnsDefaultForEmpty()
        {
            Assert.Equal(0, await Enumerable.Empty<int>().AsQueryable().FirstOrDefaultAsync());
        }

        [Fact]
        public async Task FirstOrDefaultAsyncWithPredicateReturnsDefaultForNoMatch()
        {
            Assert.Equal(0, await Source.FirstOrDefaultAsync(x => x > 100));
        }

        [Fact]
        public async Task LastAsyncReturnsLastElement()
        {
            Assert.Equal(5, await Source.LastAsync());
        }

        [Fact]
        public async Task LastAsyncWithPredicateReturnsLastMatch()
        {
            Assert.Equal(4, await Source.LastAsync(x => x < 5));
        }

        [Fact]
        public async Task LastOrDefaultAsyncReturnsDefaultForEmpty()
        {
            Assert.Equal(0, await Enumerable.Empty<int>().AsQueryable().LastOrDefaultAsync());
        }

        [Fact]
        public async Task SingleAsyncReturnsTheOnlyElement()
        {
            Assert.Equal(42, await new[] { 42 }.AsQueryable().SingleAsync());
        }

        [Fact]
        public async Task SingleAsyncWithPredicateReturnsMatch()
        {
            Assert.Equal(3, await Source.SingleAsync(x => x == 3));
        }

        [Fact]
        public async Task SingleOrDefaultAsyncReturnsDefaultForEmpty()
        {
            Assert.Equal(0, await Enumerable.Empty<int>().AsQueryable().SingleOrDefaultAsync());
        }

        [Fact]
        public async Task ElementAtAsyncReturnsItemAtIndex()
        {
            Assert.Equal(3, await Source.ElementAtAsync(2));
        }

        [Fact]
        public async Task ElementAtOrDefaultAsyncReturnsDefaultForOutOfRange()
        {
            Assert.Equal(0, await Source.ElementAtOrDefaultAsync(99));
        }

        [Fact]
        public async Task ContainsAsyncReturnsTrueForPresent()
        {
            Assert.True(await Source.ContainsAsync(3));
        }

        [Fact]
        public async Task ContainsAsyncReturnsFalseForAbsent()
        {
            Assert.False(await Source.ContainsAsync(99));
        }

        [Fact]
        public async Task ContainsAsyncWithComparerReturnsTrueForPresent()
        {
            Assert.True(await Source.ContainsAsync(3, EqualityComparer<int>.Default));
        }

        [Fact]
        public async Task SequenceEqualAsyncReturnsTrueForMatch()
        {
            Assert.True(await Source.SequenceEqualAsync(new[] { 1, 2, 3, 4, 5 }));
        }

        [Fact]
        public async Task SequenceEqualAsyncReturnsFalseForMismatch()
        {
            Assert.False(await Source.SequenceEqualAsync(new[] { 1, 2, 3 }));
        }

        [Fact]
        public async Task AnyAsyncReturnsTrueForNonEmpty()
        {
            Assert.True(await Source.AnyAsync());
        }

        [Fact]
        public async Task AnyAsyncReturnsFalseForEmpty()
        {
            Assert.False(await Enumerable.Empty<int>().AsQueryable().AnyAsync());
        }

        [Fact]
        public async Task AnyAsyncWithPredicate()
        {
            Assert.True(await Source.AnyAsync(x => x > 4));
            Assert.False(await Source.AnyAsync(x => x > 100));
        }

        [Fact]
        public async Task AllAsyncWithPredicate()
        {
            Assert.True(await Source.AllAsync(x => x > 0));
            Assert.False(await Source.AllAsync(x => x > 2));
        }

        [Fact]
        public async Task CountAsyncReturnsCount()
        {
            Assert.Equal(5, await Source.CountAsync());
        }

        [Fact]
        public async Task CountAsyncWithPredicate()
        {
            Assert.Equal(3, await Source.CountAsync(x => x > 2));
        }

        [Fact]
        public async Task LongCountAsyncReturnsCount()
        {
            Assert.Equal(5L, await Source.LongCountAsync());
        }

        [Fact]
        public async Task LongCountAsyncWithPredicate()
        {
            Assert.Equal(2L, await Source.LongCountAsync(x => x > 3));
        }

        [Fact]
        public async Task MinAsyncReturnsMin()
        {
            Assert.Equal(1, await Source.MinAsync());
        }

        [Fact]
        public async Task MinAsyncWithSelector()
        {
            Assert.Equal(-5, await Source.MinAsync(x => -x));
        }

        [Fact]
        public async Task MaxAsyncReturnsMax()
        {
            Assert.Equal(5, await Source.MaxAsync());
        }

        [Fact]
        public async Task MaxAsyncWithSelector()
        {
            Assert.Equal(10, await Source.MaxAsync(x => x * 2));
        }

        [Fact]
        public async Task SumAsyncIntReturnsSum()
        {
            Assert.Equal(15, await Source.SumAsync());
        }

        [Fact]
        public async Task SumAsyncWithSelector()
        {
            Assert.Equal(30, await Source.SumAsync(x => x * 2));
        }

        [Fact]
        public async Task SumAsyncDecimal()
        {
            Assert.Equal(6m, await new[] { 1m, 2m, 3m }.AsQueryable().SumAsync());
        }

        [Fact]
        public async Task SumAsyncDouble()
        {
            Assert.Equal(6.0, await new[] { 1.0, 2.0, 3.0 }.AsQueryable().SumAsync());
        }

        [Fact]
        public async Task AverageAsyncInt()
        {
            Assert.Equal(3.0, await Source.AverageAsync());
        }

        [Fact]
        public async Task AverageAsyncDecimal()
        {
            Assert.Equal(2m, await new[] { 1m, 2m, 3m }.AsQueryable().AverageAsync());
        }
    }
}
