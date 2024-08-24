namespace InMemoryEventStore.Abstractions
{
    public abstract class StorageEngine
    {
        protected static readonly Dictionary<string, EventStream> _eventStreams = [];

        public abstract void Append(string streamId, object @event);

        public EventStream Load(string streamId)
        {
            var streamExists = _eventStreams.TryGetValue(streamId, out var stream);
            if (!streamExists) throw new Exception($"Stream {streamId} does not exist!");
            return stream;
        }
    }
}
