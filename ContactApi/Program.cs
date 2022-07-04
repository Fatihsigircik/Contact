using System.Text.Json.Serialization;
using DbContext;
using KafkaInfrastructure;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ContactContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")),ServiceLifetime.Singleton);
builder.Services.AddSingleton<IHostedService, KafkaInfrastructure.Consumer>(q =>
{
  
        var dbContext = q.GetService<ContactContext>();
        return new Consumer(){Context = dbContext};
    
});

builder.Services.AddControllers().AddJsonOptions(x =>
    x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
builder.Services.AddControllers();


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



var app = builder.Build();
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
