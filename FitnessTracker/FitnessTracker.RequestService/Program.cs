using FitnessTracker.RequestService.Extensions;
using FitnessTracker.Core.Abstractions;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    // ƒобавление метаданных API
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Version = "v1",
        Title = "Request API",
        Description = "—ервис-посредник дл€ перенаправлени€ запросов в очереди брокера"
    });
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

builder.Services.ConfigureServices();

var app = builder.Build();

var rc = app.Services.GetRequiredService<IClient>();
await rc.InitServiceAsync();

rc.InitCallbackQueue("workouts-add");
rc.InitCallbackQueue("workouts-update");
rc.InitCallbackQueue("workouts-get");
rc.InitCallbackQueue("workouts-delete");
rc.InitCallbackQueue("workouts-stat");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
