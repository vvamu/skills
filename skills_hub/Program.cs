using EmailProvider.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using skills_hub.core.Helpers;
using skills_hub.core.Options;
using skills_hub.core.repository;
using skills_hub.core.Repository;
using skills_hub.core.Repository.User;
using skills_hub.domain.Models.User;
using skills_hub.persistence;
using System.Security.Claims;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews().
            AddNewtonsoftJson(options =>
            options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);


builder.Services.AddSession(options =>
{


    options.Cookie.Name = ".AdventureWorks.Session";
    options.IdleTimeout = TimeSpan.FromDays(10);
    options.Cookie.IsEssential = true;
});


var services = builder.Services;
services.AddAutoMapper(typeof(MappingProfile).Assembly);

#region Options pattern
IConfiguration configuration = new ConfigurationBuilder()
.SetBasePath(Directory.GetCurrentDirectory())
.AddJsonFile("appsettings.json")
.Build();

builder.Services.AddSingleton(configuration);
services.Configure<AdminOptions>(configuration.GetSection(AdminOptions.OptionName));
services.Configure<ConnectionStringsOptions>(configuration.GetSection(ConnectionStringsOptions.OptionName));
services.Configure<MailOptions>(configuration.GetSection(MailOptions.OptionName));

#endregion

#region DbConfig

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
services.AddDbContext<ApplicationDbContext>(
options =>
{
    options.UseSqlServer(connectionString, options => options.EnableRetryOnFailure().CommandTimeout(60));
    options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
    options.EnableSensitiveDataLogging();
},
ServiceLifetime.Scoped);

#endregion

#region Authentication

builder.Services.AddIdentity<ApplicationUser, IdentityRole<Guid>>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();


builder.Services.ConfigureApplicationCookie(options =>
{
    options.ExpireTimeSpan = TimeSpan.FromDays(30);
});

#endregion

#region Roles
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Teacher", builderPolicy =>
    {
        builderPolicy.RequireClaim(ClaimTypes.Role, "Teacher");
    });
    options.AddPolicy("Student", builderPolicy =>
    {
        builderPolicy.RequireClaim(ClaimTypes.Role, "Student");
    });

    options.AddPolicy("Admin", builderPolicy =>
    {
        builderPolicy.RequireAssertion(x => x.User.HasClaim(ClaimTypes.Role, "Teacher") ||
                                       x.User.HasClaim(ClaimTypes.Role, "Admin"));
    });


});

#endregion

builder.Services.AddTransient<EmailProvider.Interfaces.IMailService, EmailProvider.MailService>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IGroupService, GroupService>();
builder.Services.AddTransient<ILessonService, LessonService>();
builder.Services.AddScoped<IAbstractLogModel<BaseUserInfo>, BaseUserInfoService>();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
