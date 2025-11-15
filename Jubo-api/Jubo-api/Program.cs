using Jubo_api.Interfaces.BusinessLogic;
using jubo_api.Interfaces.Storage;
using Jubo_api.Services.BusinessLogic;
using Jubo_api.Services.Storage;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.AddConsole();
builder.Services.AddHttpLogging(o => { });

var mongodbSection = builder.Configuration.GetSection("MongoDb");


builder.Services.AddSingleton<IMongoClient>(_ => new MongoClient(mongodbSection["ConnectionString"]));
builder.Services.AddScoped(sp =>
{
    var client = sp.GetRequiredService<IMongoClient>();
    var database = "JuboDb";
    return client.GetDatabase(database);
});

// Add services to the container.
builder.Services.AddScoped<IPatientsBusinessLogic, PatientsBusinessLogic>();
builder.Services.AddScoped<IOrderBusinessLogic, OrderBusinessLogic>();
builder.Services.AddScoped<IPatientsStorage, PatientsStorage>();
builder.Services.AddScoped<IOrderStorage, OrderStorage>();
builder.Services.AddScoped<ICounterStorage, CounterStorage>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(corsPolicyBuilder =>
    {
        corsPolicyBuilder.AllowAnyOrigin();
        corsPolicyBuilder.AllowAnyHeader();
        corsPolicyBuilder.AllowAnyMethod();
    });
});

var app = builder.Build();
app.UseHttpLogging();
app.UseCors();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Configure the HTTP request pipeline.

// app.UseHttpsRedirection();
//
// app.UseAuthorization();

app.MapControllers();

app.Run();