using MassTransit;
using Shared;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMassTransit(opt =>
{
    //opt.AddConsumer<StockReservedEventConsumer>();

    //opt.UsingRabbitMq((context, cfg) =>
    //{
    //    cfg.Host(builder.Configuration.GetConnectionString("RabbitMq"));
    //    cfg.ReceiveEndpoint(RabbitMqSettingsConst.StockReservedEventQueueName, e =>
    //    {
    //        e.ConfigureConsumer<StockReservedEventConsumer>(context);
    //    });
    //});
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
