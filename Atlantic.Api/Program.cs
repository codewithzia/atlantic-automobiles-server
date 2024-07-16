using Atlantic.Data.Hubs;
using Atlantic.Data.Models.Auth;
using Atlantic.Data.Models.Settings;
using Atlantic.Data.Models.System;
using Atlantic.Data.MongoFs;
using Atlantic.Data.Repositories;
using Atlantic.Data.Seeds;
using Atlantic.Data.Services;
using Atlantic.Data.Services.Auth;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using MongoDB.Entities;
using Newtonsoft.Json.Serialization;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var Configuration = builder.Configuration;

builder.Services.AddSignalR();


// Add services to the container.
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
        .AddMongoDbStores<ApplicationUser, ApplicationRole, Guid>
        (
          builder.Configuration.GetSection("DbSettings:ConnectionStrings").Value, builder.Configuration.GetSection("DbSettings:Database:Name").Value
        //"mongodb://useradmin:adminPa55@137.59.51.10:27017/?authSource=admin", "ebsAuth"      
        )
        .AddTokenProvider<DataProtectorTokenProvider<ApplicationUser>>(TokenOptions.DefaultProvider);

builder.Services.AddSignalR();
builder.Services.Configure<MongoSettings>(Configuration.GetSection(nameof(MongoSettings)));
builder.Services.Configure<DbSettings>(builder.Configuration.GetSection("DbSettings"));
builder.Services.AddSingleton<IMongoSettings>(sp => sp.GetRequiredService<IOptions<MongoSettings>>().Value);
builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));
builder.Services.AddTransient<IMailService, MailService>();

builder.Services.Configure<IdentityOptions>(options =>
{
    // Default Password settings.
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 3;
    options.Password.RequiredUniqueChars = 1;
});


builder.Services.AddCors(c =>
{
    c.AddPolicy("AllowOrigin", options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

});

var config = new MapperConfiguration(cfg =>
{
    cfg.AddProfile(new AutoMapperProfile());
});
var mapper = config.CreateMapper();
builder.Services.AddSingleton(mapper);

builder.Services.AddScoped<IHttpContextAccessorService, HttpContextAccessorService>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IOtpService, OtpService>();
builder.Services.AddScoped<ICountryService, CountryService>();
builder.Services.AddSingleton<IConnectionService, ConnectionService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthService, AuthService>();

//JSON Serilizer
builder.Services.AddControllersWithViews().AddNewtonsoftJson(options =>
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore)
    .AddNewtonsoftJson(o => o.SerializerSettings.ContractResolver
    = new CamelCasePropertyNamesContractResolver());

//FILE Service
builder.Services.Configure<FilestoreDatabaseSettings>(
             builder.Configuration.GetSection(nameof(FilestoreDatabaseSettings)));
builder.Services.AddSingleton<IFilestoreDatabaseSettings>(sp => sp.GetRequiredService<IOptions<FilestoreDatabaseSettings>>().Value);
builder.Services.AddSingleton<FilesService>();

builder.Services.AddControllers().AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);



// configure strongly typed settings objects
var appSettingsSection = Configuration.GetSection("AppSettings");
builder.Services.Configure<AppSettings>(appSettingsSection);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "Demo API", Version = "v1" });
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8
                .GetBytes(builder.Configuration.GetSection("AppSettings:Token").Value)),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

builder.Services.AddScoped<IAppSeederInitial, AppSeederInitial>();
builder.Services.AddScoped<IKeySetup, KeySetup>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
app.UseSwaggerUI();
//}

// Enable CQRS
app.UseCors(options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());


var settings = app.Services.GetRequiredService<IOptions<DbSettings>>().Value;
await DB.InitAsync(settings.Database.Name, MongoClientSettings.FromConnectionString(settings.ConnectionStrings));


app.UseDefaultFiles();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseHttpsRedirection();

app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});
app.MapHub<NotificationHub>("/hub");

SeedDatabase();

app.UseSpa(spa => { });

app.Run();


async void SeedDatabase() //can be placed at the very bottom under app.Run()
{
    using (var scope = app.Services.CreateScope())
    {
        var appSeeder = scope.ServiceProvider.GetRequiredService<IAppSeederInitial>();
        await appSeeder.Initialize();
        await scope.ServiceProvider.GetRequiredService<IKeySetup>().Setup();
    }
}