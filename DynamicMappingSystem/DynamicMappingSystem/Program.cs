using DynamicMappingSystem.Application.Interfaces;
using DynamicMappingSystem.Application.Mapping;
using DynamicMappingSystem.Application.Validation;
using DynamicMappingSystem.Configuration;
using DynamicMappingSystem.Infrastructure.Providers;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<MappingRulesSettings>(
    builder.Configuration.GetSection("MappingRules"));

builder.Services.AddSingleton<JsonMappingRuleProvider>(provider =>
{
    var settings = provider.GetRequiredService<IOptions<MappingRulesSettings>>().Value;
    return new JsonMappingRuleProvider(settings.JsonMappingRuleFilePath);
});

builder.Services.AddSingleton<MongoMappingRuleProvider>();

builder.Services.AddSingleton<IMappingRuleProviderFactory, MappingRuleProviderFactory>();

builder.Services.AddSingleton<IMappingRuleProvider>(provider =>
{
    var factory = provider.GetRequiredService<IMappingRuleProviderFactory>();
    return factory.GetProvider();
});

builder.Services.AddSingleton<JsonFormatConfigProvider>(provider =>
{
    var settings = provider.GetRequiredService<IOptions<MappingRulesSettings>>().Value;
    return new JsonFormatConfigProvider(settings.JsonDataFormatFilePath);
});

builder.Services.AddSingleton<MongoFormatConfigProvider>();

builder.Services.AddSingleton<IFormatConfigProviderFactory, FormatConfigProviderFactory>();

builder.Services.AddSingleton<IFormatConfigProvider>(provider =>
{
    var factory = provider.GetRequiredService<IFormatConfigProviderFactory>();
    return factory.GetProvider();
});

builder.Services.AddScoped<IModelValidator, DataFormatValidator>();
builder.Services.AddScoped<IMapHandler, MapHandler>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

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
