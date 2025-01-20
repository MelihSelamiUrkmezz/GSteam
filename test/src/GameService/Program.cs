using GameService.Base;
using GameService.Consumers;
using GameService.Data;
using GameService.Repositories;
using GameService.Repositories.ForCategory;
using GameService.Repositories.ForGameImage;
using GameService.Services;
using MassTransit;
using MassTransit.EntityFrameworkCoreIntegration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddDbContext<GameDatabaseContext>(opt => {
    opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddScoped(typeof(BaseResponseModel));
builder.Services.AddScoped<ICategoryRepository,CategoryRepository>();
builder.Services.AddScoped<IGameRepository,GameRepository>();
builder.Services.AddScoped<IFileService,FileService>();
builder.Services.AddScoped<IGameImageRepository,GameImageRepository>(); 
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSwaggerGen();
builder.Services.Configure<ApiBehaviorOptions>(options => options.SuppressModelStateInvalidFilter = true);
builder.Services.AddMassTransit(opt => {

    opt.AddEntityFrameworkOutbox<GameDatabaseContext>(x=>{
        x.LockStatementProvider = new PostgresLockStatementProvider();
        x.QueryDelay = TimeSpan.FromSeconds(10);

        x.UsePostgres();
        x.UseBusOutbox();
    });
    opt.AddConsumersFromNamespaceContaining<GameCreatedFaultConsumer>();

    opt.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("game",false));
    opt.UsingRabbitMq((context,cfg) => {
        cfg.Host(builder.Configuration["RabbitMQ:Host"],"/",host => {
            host.Username(builder.Configuration.GetValue("RabbitMQ:Username","guest"));
            host.Username(builder.Configuration.GetValue("RabbitMQ:Password","guest"));
        });
        cfg.ConfigureEndpoints(context);
    });
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options => {
    options.Authority = builder.Configuration["AuthorirtyServiceUrl"];
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters.ValidateAudience = false;
    options.TokenValidationParameters.NameClaimType = "username";
});
builder.Services.AddGrpc();

// CORS politikasını ekle
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(); // CORS middleware'ini ekle

// Statik dosya servisini etkinleştir
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapGrpcService<GrpcGameService>();
app.MapGrpcService<GrpcMyGameService>();

ApplyPendigMigration();

// Seed data'yı çağır
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<GameDatabaseContext>();
    await SeedData.SeedCategories(context);
}

app.Run();



void ApplyPendigMigration()
{
    using var scope = app.Services.CreateScope();

    var _db = scope.ServiceProvider.GetRequiredService<GameDatabaseContext>();

    if (_db.Database.GetPendingMigrations().Count() > 0)
        _db.Database.Migrate();
}
