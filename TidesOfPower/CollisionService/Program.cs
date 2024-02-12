using CollisionService.Interfaces;
using CollisionService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IConsumerService, ConsumerService>();
builder.Services.AddHostedService<IConsumerService>(provider =>
    provider.GetService<IConsumerService>() ?? new ConsumerService());

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHttpsRedirection();
}

app.UseAuthorization();

app.MapControllers();

app.Run();