using MasterNet.Application;
using MasterNet.Application.Interfaces;
using MasterNet.Infrastructure;
using MasterNet.Infrastructure.Photos;
using MasterNet.Persistence;
using MasterNet.WebApi.Extensions;
using MasterNet.WebApi.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure();
builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddIdentityServices(builder.Configuration);

builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("Cloudinary"));

builder.Services.AddScoped<IPhotoService, PhotoService>();

builder.Services.AddHttpContextAccessor();


builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddOpenApi();

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();
await app.SeedDataAuthenticationAsync();

app.UseHttpsRedirection();
app.MapControllers();

app.Run();