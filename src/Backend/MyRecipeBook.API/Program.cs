using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MyRecipeBook.API.Converters;
using MyRecipeBook.API.Filters;
using MyRecipeBook.Application;
using MyRecipeBook.Domain.Extensions;
using MyRecipeBook.Domain.Repositories.User;
using MyRecipeBook.Infraestructure;
using MyRecipeBook.Infraestructure.Migrations;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

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

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    var signingKey = builder.Configuration.GetValue<string>("Jwt:SigningKey")!;

    options.TokenValidationParameters = new()
    {
        ValidateAudience = false,
        ValidateIssuer = false,
        ValidateIssuerSigningKey = true,
        ValidateLifetime = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey)),
        ClockSkew = TimeSpan.Zero
    };

    options.Events = new JwtBearerEvents
    {
        OnTokenValidated = async context =>
        {
            var userId = context.Principal?.FindFirstValue(JwtRegisteredClaimNames.Sub) ?? context.Principal?.FindFirstValue(ClaimTypes.NameIdentifier);

            if(userId.IsEmpty())
            {
                context.Fail("Invalid subject");
                context.HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return;
            }

            var userRepository = context.HttpContext.RequestServices.GetRequiredService<IUserReadOnlyRepository>();
            var existingUser = await userRepository.ExistActiveUserWithId(Guid.Parse(userId));

            if (!existingUser)
            {
                context.Fail("Invalid user");
                context.HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
            }
        }
    };

});

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

await ExecuteMigrations();

app.Run();

async Task ExecuteMigrations() {

    await using var scope = app.Services.CreateAsyncScope();

    await DatabaseMigration.ExecuteMigrations(scope.ServiceProvider);

}

public partial class Program { }
