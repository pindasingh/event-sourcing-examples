namespace InMemoryEventStore.Abstractions;

public class EventStream
{
    public EventStream(string streamId)
    {
        StreamId = streamId;
    }

    public IEnumerable<object> Events { get; set; }

    public int Sequence { get; set; } = 0;

    public string StreamId { get; private set; }
}
