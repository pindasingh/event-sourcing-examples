using InMemoryEventStore.Abstractions;
using InMemoryEventStore.Storage;
using NUnit.Framework;

namespace InMemoryEventStore.UnitTests;

[TestFixture]
public class PessimisticInMemoryStorageEngineUnitTests
{
    private readonly StorageEngine _storageEngine;

    public PessimisticInMemoryStorageEngineUnitTests()
    {
        _storageEngine = new PessimisticInMemoryStorageEngine();
    }

    [Test]
    public void WhenNonExistentStreamId_ThenExceptionIsThrown()
    {
        var streamId = Guid.NewGuid().ToString();

        Assert.Throws<Exception>(() => _storageEngine.Load(streamId));
    }

    [Test]
    public void WhenSingleEventAppeneded_ThenItIsSuccessfullyRetreived()
    {
        var streamId = Guid.NewGuid().ToString();

        _storageEngine.Append(streamId, null);

        var stream = _storageEngine.Load(streamId);

        Assert.That(stream.Events.Count, Is.EqualTo(1));
        Assert.That(stream.Sequence, Is.EqualTo(1));
    }

    [Test]
    public void WhenMultipleEventsAppeneded_ThenAllAreSuccessfullyRetreived()
    {
        var streamId = Guid.NewGuid().ToString();

        _storageEngine.Append(streamId, null);
        _storageEngine.Append(streamId, null);

        var stream = _storageEngine.Load(streamId);

        Assert.That(stream.Events.Count, Is.EqualTo(2));
        Assert.That(stream.Sequence, Is.EqualTo(2));
    }

    [Test]
    public void WhenMultipleConcurrentEventsAppended_ThenAllAreSuccessfullyAppended()
    {
        var streamId = Guid.NewGuid().ToString();
        int numberOfEvents = 10;

        var tasks = new List<Task>();
        for (int i = 0; i < numberOfEvents; i++)
        {
            tasks.Add(Task.Run(() => _storageEngine.Append(streamId, null)));
        }
        Task.WaitAll(tasks.ToArray());

        var stream = _storageEngine.Load(streamId);

        Assert.That(stream.Events.Count, Is.EqualTo(numberOfEvents));
        Assert.That(stream.Sequence, Is.EqualTo(numberOfEvents));
    }
}
