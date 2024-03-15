using ProjectileService.Interfaces;

var builder = WebApplication.CreateBuilder(args);

//https://github.com/dotnet/runtime/issues/36063
builder.Services.AddSingleton<IConsumerService, ProjectileService.Services.ProjectileService>();
builder.Services.AddHostedService(provider => provider.GetRequiredService<IConsumerService>());

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