using Orders.Api.Infrastructure;
using Orders.Application;
using Orders.Infrastructure;
using Orders.Infrastructure.Persistence;

const string CorsPolicyName = "ClientApp";

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration);

builder.Services
    .AddControllers(options => options.Filters.Add<FluentValidationActionFilter>())
    .ConfigureApiBehaviorOptions(options =>
        options.InvalidModelStateResponseFactory = context =>
            new ProblemDetailsResult(ValidationProblemDetailsFactory.Create(context)));

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => options.SwaggerDoc("v1", new() { Title = "Orders API", Version = "v1" }));

var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];

builder.Services.AddCors(options => options.AddPolicy(
    CorsPolicyName,
    policy => policy.WithOrigins(allowedOrigins).AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    await DatabaseInitializer.MigrateAsync(app.Services, app.Lifetime.ApplicationStopping);
}

app.UseCors(CorsPolicyName);
app.MapControllers();

await app.RunAsync();
