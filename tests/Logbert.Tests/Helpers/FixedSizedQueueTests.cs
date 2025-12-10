using Xunit;
using AwesomeAssertions;
using System.Reflection;

namespace Logbert.Tests.Helpers;

/// <summary>
/// Unit tests for FixedSizedQueue class.
/// </summary>
public class FixedSizedQueueTests
{
    // Note: FixedSizedQueue is internal, so we use reflection to test it
    // or create a wrapper. For now, testing through the public assembly.

    [Fact]
    public async Task FixedSizedQueueShouldInitializeWithCorrectSize()
    {
        // Arrange & Act
        var queue = CreateFixedSizedQueue<int>(5);

        // Assert
        var size = await Task.FromResult(GetQueueSize(queue));
        size.Should().Be(5);
    }

    [Fact]
    public async Task FixedSizedQueueShouldEnqueueItemsSuccessfully()
    {
        // Arrange
        var queue = CreateFixedSizedQueue<string>(3);

        // Act
        EnqueueItem(queue, "item1");
        EnqueueItem(queue, "item2");
        var count = await Task.FromResult(GetQueueCount(queue));

        // Assert
        count.Should().Be(2);
    }

    [Fact]
    public async Task FixedSizedQueueShouldNotExceedMaximumSize()
    {
        // Arrange
        var queue = CreateFixedSizedQueue<int>(3);

        // Act
        EnqueueItem(queue, 1);
        EnqueueItem(queue, 2);
        EnqueueItem(queue, 3);
        EnqueueItem(queue, 4);
        EnqueueItem(queue, 5);
        var count = await Task.FromResult(GetQueueCount(queue));

        // Assert
        count.Should().Be(3);
    }

    [Fact]
    public async Task FixedSizedQueueShouldRemoveOldestItemWhenFull()
    {
        // Arrange
        var queue = CreateFixedSizedQueue<int>(3);

        // Act
        EnqueueItem(queue, 1);
        EnqueueItem(queue, 2);
        EnqueueItem(queue, 3);
        EnqueueItem(queue, 4); // Should remove 1

        var items = await Task.FromResult(GetQueueItems(queue));

        // Assert
        items.Should().NotContain(1);
        items.Should().Contain(4);
    }

    [Fact]
    public async Task FixedSizedQueueShouldMaintainFifoOrder()
    {
        // Arrange
        var queue = CreateFixedSizedQueue<int>(5);

        // Act
        EnqueueItem(queue, 1);
        EnqueueItem(queue, 2);
        EnqueueItem(queue, 3);
        var items = await Task.FromResult(GetQueueItems(queue));

        // Assert
        items.Should().ContainInOrder(1, 2, 3);
    }

    [Fact]
    public async Task FixedSizedQueueShouldHandleSizeOfOne()
    {
        // Arrange
        var queue = CreateFixedSizedQueue<string>(1);

        // Act
        EnqueueItem(queue, "first");
        EnqueueItem(queue, "second");
        var count = await Task.FromResult(GetQueueCount(queue));
        var items = await Task.FromResult(GetQueueItems(queue));

        // Assert
        count.Should().Be(1);
        items.Should().Contain("second");
        items.Should().NotContain("first");
    }

    [Fact]
    public async Task FixedSizedQueueShouldHandleEmptyQueue()
    {
        // Arrange
        var queue = CreateFixedSizedQueue<int>(5);

        // Act
        var count = await Task.FromResult(GetQueueCount(queue));

        // Assert
        count.Should().Be(0);
    }

    [Fact]
    public async Task FixedSizedQueueShouldPreserveSizeProperty()
    {
        // Arrange
        var queue = CreateFixedSizedQueue<int>(10);

        // Act
        EnqueueItem(queue, 1);
        EnqueueItem(queue, 2);
        var size = await Task.FromResult(GetQueueSize(queue));

        // Assert
        size.Should().Be(10);
    }

    #region Helper Methods

    private static object CreateFixedSizedQueue<T>(int size)
    {
        var assembly = typeof(Couchcoding.Logbert.Helper.StringExtensions).Assembly;
        var type = assembly.GetType("Couchcoding.Logbert.Helper.FixedSizedQueue`1");
        var genericType = type!.MakeGenericType(typeof(T));
        return Activator.CreateInstance(genericType, size)!;
    }

    private static void EnqueueItem<T>(object queue, T item)
    {
        var method = queue.GetType().GetMethod("Enqueue");
        method!.Invoke(queue, new object?[] { item });
    }

    private static int GetQueueCount(object queue)
    {
        var property = queue.GetType().GetProperty("Count");
        return (int)property!.GetValue(queue)!;
    }

    private static int GetQueueSize(object queue)
    {
        var property = queue.GetType().GetProperty("Size");
        return (int)property!.GetValue(queue)!;
    }

    private static List<T> GetQueueItems<T>(object queue)
    {
        var toArrayMethod = queue.GetType().GetMethod("ToArray");
        var array = (T[])toArrayMethod!.Invoke(queue, null)!;
        return array.ToList();
    }

    #endregion
}
