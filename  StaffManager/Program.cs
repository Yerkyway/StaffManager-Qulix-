using StaffManager.DataAccess;
using StaffManager.DataAccess.RepositoryImplementation;
using StaffManager.Repository;
using StaffManager.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<DatabaseConnection>(provider => 
    new DatabaseConnection(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<ICompanyRepository, CompanyRepositoryImplementation>();
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepositoryImplementation>();
builder.Services.AddScoped<CompanyService>();
builder.Services.AddScoped<EmployeeService>();

var app = builder.Build();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Home}/{id?}");

app.Run();