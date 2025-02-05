using EventSourcing.Shared.Interfaces;
using EventStore.ClientAPI;
using System.Text;
using System.Text.Json;

namespace EventSourcing.API.EventStores
{
    public abstract class AbstractStream
    {
        protected readonly LinkedList<IEvent> Events = new LinkedList<IEvent>();

        private string StreamName { get;}
        private readonly IEventStoreConnection _eventStoreConnection;

        protected AbstractStream(string streamName, IEventStoreConnection eventStoreConnection)
        {
            StreamName = streamName;
            _eventStoreConnection = eventStoreConnection;
        }

        public async Task SaveAsync()
        {
            var newEvents = Events.ToList()
                .Select(x=> new EventData(
                    Guid.NewGuid(),
                    x.GetType().Name,
                    true,
                    Encoding.UTF8.GetBytes(JsonSerializer.Serialize(x, inputType: x.GetType())),
                    Encoding.UTF8.GetBytes(x.GetType().Name))).ToList();

            await _eventStoreConnection.AppendToStreamAsync(StreamName, ExpectedVersion.Any,newEvents);

            Events.Clear();
        }
    }
}
