using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;
using MyRecipeBook.API.Converters;
using MyRecipeBook.API.Filters;
using MyRecipeBook.Application;
using MyRecipeBook.Infraestructure;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

/// :: Add services to the container.
builder.Services.AddControllers().AddJsonOptions(options =>
{
    /// :: Custom string converter to handle string serialization and deserialization.
    options.JsonSerializerOptions.Converters.Add(new StringConverter());
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

/// :: Dependecy injections.
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

/// :: Configure localization options for bilingual responses.
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new List<CultureInfo> { new("en"), new("pt-BR"), new("es") };

    options.DefaultRequestCulture = new RequestCulture("en");

    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;

    options.RequestCultureProviders = [ new AcceptLanguageHeaderRequestCultureProvider() ];
});

builder.Services.AddMvc(options => options.Filters.Add<ExceptionFilter>());

builder.Services.AddRouting(options => options.LowercaseUrls = true);

var app = builder.Build();

/// :: Configure localization middleware.
var localizationOptions = app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>();

/// :: Use the configured localization options in the request pipeline.
app.UseRequestLocalization(localizationOptions.Value);

/// :: Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
