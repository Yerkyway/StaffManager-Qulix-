using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using StaffManager.Models;
using StaffManager.Services;

namespace StaffManager.Controllers;

public class HomeController : Controller
{
    private readonly CompanyService _companyService;
    private readonly EmployeeService _employeeService;

    public HomeController(CompanyService companyService, EmployeeService employeeService)
    {
        _companyService = companyService;
        _employeeService = employeeService;
    }
    

    public async Task<IActionResult> Home()
    {
        try
        {
            var companies = await _companyService.GetAllCompaniesAsync();
            var employees = await _employeeService.GetAllEmployeesAsync();

            var dashboardView = new DashboardViewModel
            {
                totalCompanies = companies.Count,
                totalEmployees = employees.Count,
                companiesWithEmployees = companies.Count(c => c.NumberOfEmployees > 0),
                recentEmployees = employees.OrderByDescending(e => e.HireDate).Take(5).ToList()
            };

            return View(typeof(DashboardViewModel));
        }
        catch (Exception e)
        {
            ViewBag.ErrorMessage = "An error occurred while loading the dashboard. Please try again later.";
            return View(new DashboardViewModel());
        }
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        });
    }
}