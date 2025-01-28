using MassTransit;
using Microsoft.EntityFrameworkCore;
using Order.API.Models;
using Shared;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMassTransit(opt =>
{
    //opt.AddConsumer<PaymentCompletedEventConsumer>();
    //opt.AddConsumer<PaymentFailedEventConsumer>();
    //opt.AddConsumer<StockNotReservedEventConsumer>();

    opt.UsingRabbitMq((context, cfg) =>
    {

        cfg.Host(builder.Configuration.GetConnectionString("RabbitMq"));
        //cfg.ReceiveEndpoint(RabbitMqSettingsConst.OrderPaymentCompletedEventQueueName, e =>
        //{
        //    e.ConfigureConsumer<PaymentCompletedEventConsumer>(context);
        //});
        //cfg.ReceiveEndpoint(RabbitMqSettingsConst.OrderPaymentFailedEventQueueName, e =>
        //{
        //    e.ConfigureConsumer<PaymentFailedEventConsumer>(context);
        //});
        //cfg.ReceiveEndpoint(RabbitMqSettingsConst.OrderStockNotReservedEventQueueName, e =>
        //{
        //    e.ConfigureConsumer<StockNotReservedEventConsumer>(context);
        //});

    });
});

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlConn"));
});
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();