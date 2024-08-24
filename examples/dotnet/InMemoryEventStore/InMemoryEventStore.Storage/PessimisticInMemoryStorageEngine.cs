using InMemoryEventStore.Abstractions;

namespace InMemoryEventStore.Storage;

public class PessimisticInMemoryStorageEngine : StorageEngine
{
    private static readonly SemaphoreSlim _semaphore = new(1, 1);

    public override void Append(string streamId, object @event)
    {
        try
        {
            _semaphore.Wait();

            var streamExists = _eventStreams.TryGetValue(streamId, out var eventStream);
            if (!streamExists)
            {
                _eventStreams.Add(streamId, new EventStream(streamId)
                {
                    Events = [@event],
                    Sequence = 1
                });
                return;
            }

            _eventStreams[streamId].Sequence++;
            var events = eventStream.Events.Append(@event);
            _eventStreams[streamId].Events = events;
        }
        finally
        {
            _semaphore.Release();
        }
    }
}
