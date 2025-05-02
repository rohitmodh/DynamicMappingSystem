using DynamicMappingSystem.Converters;
using DynamicMappingSystem.Mapping;
using DynamicMappingSystem.Providers;
using DynamicMappingSystem.Validation;
using FluentValidation;
using System;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IMappingRuleProvider>(provider =>
    new JsonMappingRuleProvider("MappingRules/mapping-rules.json"));

builder.Services.AddValidatorsFromAssemblyContaining<ReservationValidator>();

builder.Services.AddScoped<IFormatConfigProvider>(provider =>
            new JsonFormatConfigProvider("MappingRules/DataFormat.json"));

builder.Services.AddScoped(provider =>
            new MappingConfigService("MappingRules/mapping-rules.json"));

builder.Services.AddScoped<IModelValidator, DataFormatValidator>();
builder.Services.AddScoped<IMapHandler, MapHandler>();

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

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
