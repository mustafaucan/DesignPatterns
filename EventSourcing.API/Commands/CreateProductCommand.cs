using EventSourcing.API.Dtos;
using MediatR;

namespace EventSourcing.API.Commands
{
    public class CreateProductCommand : IRequest
    {
        public CreateProductDto Product { get; set; }
    }
}
