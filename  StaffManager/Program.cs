using StaffManager.DataAccess;
using StaffManager.DataAccess.RepositoryImplementation;
using StaffManager.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<DatabaseConnection>(provider => 
    new DatabaseConnection(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<ICompanyRepository, CompanyRepositoryImplementation>();
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepositoryImplementation>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Home}/{id?}");

app.Run();