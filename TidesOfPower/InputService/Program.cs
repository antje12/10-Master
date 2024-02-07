using InputService.Interfaces;
using InputService.Services;

var builder = WebApplication.CreateBuilder(args);
//builder.Services.AddHostedService<ConsumerService>();

builder.Services.AddSingleton<IConsumerService, ConsumerService>();
builder.Services.AddHostedService<IConsumerService>(provider => provider.GetService<IConsumerService>() ?? new ConsumerService());

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
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