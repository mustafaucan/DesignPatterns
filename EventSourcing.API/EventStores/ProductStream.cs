using EventSourcing.API.Dtos;
using EventSourcing.Shared.Events;
using EventStore.Client;
using System.Text.Json;

namespace EventSourcing.API.EventStores
{
    public class ProductStream
    {
        private readonly EventStoreClient _eventStoreClient;
        private const string StreamName = "ProductStream";
        private readonly List<EventData> _events = new();

        public ProductStream(EventStoreClient eventStoreClient)
        {
            _eventStoreClient = eventStoreClient;
        }

        public void Created(CreateProductDto createProductDto)
        {
            var @event = new ProductCreatedEvent
            {
                Name = createProductDto.Name,
                Price = createProductDto.Price,
                Stock = createProductDto.Stock,
                UserId = createProductDto.UserId
            };
            var eventData = new EventData(
                Uuid.NewUuid(),
                nameof(ProductCreatedEvent),
                JsonSerializer.SerializeToUtf8Bytes(@event)
            );
            _events.Add(eventData);
        }

        public void NameChanged(ChangeProductNameDto changeProductNameDto)
        {
            var @event = new ProductNameChangedEvent
            {
                Id = changeProductNameDto.Id,
                ChangedName = changeProductNameDto.Name
            };
            var eventData = new EventData(
                Uuid.NewUuid(),
                nameof(ProductNameChangedEvent),
                JsonSerializer.SerializeToUtf8Bytes(@event)
            );
            _events.Add(eventData);
        }

        public void PriceChanged(ChangeProductPriceDto changeProductPriceDto)
        {
            var @event = new ProductPriceChangedEvent
            {
                Id = changeProductPriceDto.Id,
                ChangedPrice = changeProductPriceDto.Price
            };
            var eventData = new EventData(
                Uuid.NewUuid(),
                nameof(ProductPriceChangedEvent),
                JsonSerializer.SerializeToUtf8Bytes(@event)
            );
            _events.Add(eventData);
        }

        public void Deleted(Guid id)
        {
            var @event = new ProductDeletedEvent { Id = id };
            var eventData = new EventData(
                Uuid.NewUuid(),
                nameof(ProductDeletedEvent),
                JsonSerializer.SerializeToUtf8Bytes(@event)
            );
            _events.Add(eventData);
        }

        public async Task AppendEventsAsync()
        {
            if (_events.Any())
            {
                await _eventStoreClient.AppendToStreamAsync(
                    StreamName,
                    StreamState.Any,
                    _events
                );
                _events.Clear();
            }
        }
    }
}
