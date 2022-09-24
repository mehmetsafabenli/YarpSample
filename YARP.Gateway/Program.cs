using Serilog;
using YARP.Gateway.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Host
    .AddYarpJson()
    .UseSerilog();

var assemblyName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;

if (assemblyName != null) SerilogConfigurationHelper.Configure(assemblyName);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseEndpoints(endpoints => { endpoints.MapReverseProxy(); });

await app.RunAsync();