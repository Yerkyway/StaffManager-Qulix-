using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using StaffManager.Models;
using StaffManager.Services;

namespace StaffManager.Controllers;

public class EmployeeController : Controller
{
    
    private readonly CompanyService _companyService;
    private readonly EmployeeService _employeeService;
    
    public EmployeeController(CompanyService companyService, EmployeeService employeeService)
    {
        _companyService = companyService;
        _employeeService = employeeService;
    }


    /// <summary>
    /// Displays a list of all employees in the Staff Manager application.
    /// </summary>
    /// <returns></returns>
    public async Task<IActionResult> Home()
    {
        try
        {
            var employees = await _employeeService.GetAllEmployeesAsync();
            return View(employees);
        }
        catch (Exception e)
        {
            TempData["ErrorMessage"] = "An error occurred while loading the employees: " + e.Message;
            return View(new List<EmployeeModel>());
        }
    }

    /// <summary>
    /// Displays the details of a specific employee by their ID.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<IActionResult> Details(int id)
    {
        if (id<=0)
        {
            return BadRequest();
        }

        try
        {
            var employee = await _employeeService.GetEmployeeByIdAsync(id);
            if (employee==null)
            {
                return NotFound("Employee not found.");
            }

            var workingExperience = DateTime.Now - employee.HireDate;
            ViewBag.WorkingExperience = Math.Round(workingExperience.Days / 365.0, 1);
            return View(employee);
        }
        catch (Exception e)
        {
            TempData["ErrorMessage"] = "An error occurred while loading the employee details: " + e.Message;
            return View(new EmployeeModel());
        }
    }

    /// <summary>
    /// Displays the form to create a new employee.
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var employee = new EmployeeModel
        {
            HireDate = DateTime.Today
        };
        await PopulateDropdownAsync();
        return View(employee);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(EmployeeModel employee)
    {
        await PopulateDropdownAsync(employee.CompanyId);

        // Add the custom validation like in your Update method
        var (isValid, errors) = await _employeeService.ValidateEmployeeAsync(employee);
        if (!isValid)
        {
            foreach (var error in errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        if (!ModelState.IsValid)
        {
            // Add debugging to see what validation errors occur
            foreach (var modelError in ModelState.Values.SelectMany(v => v.Errors))
            {
                Console.WriteLine($"Validation Error: {modelError.ErrorMessage}");
            }
            return View(employee);
        }

        try
        {
            await _employeeService.CreateEmployeeAsync(employee);
            TempData["SuccessMessage"] = "Employee created successfully.";
            return RedirectToAction(nameof(Home));
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error creating employee: {e.Message}");
            ModelState.AddModelError("", $"Error creating employee: {e.Message}");
            return View(employee);
        }
    }



    /// <summary>
    /// Populates the dropdown list of companies for the employee form.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> Update(int id)
    {
        if (id<=0)
        {
            return BadRequest();
        }

        try
        {
            var employee = await _employeeService.GetEmployeeByIdAsync(id);
            if (employee==null)
            {
                return NotFound("Employee not found.");
            }
            
            
            await PopulateDropdownAsync(employee.CompanyId);

            return View(employee);
        }
        catch (Exception e)
        {
            TempData["ErrorMessage"] = "An error occurred while loading the employee for update: " + e.Message;
            return RedirectToAction(nameof(Home));
        }
    }

    /// <summary>
    /// Handles the submission of the form to update an existing employee.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="employee"></param>
    /// <returns></returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(int id, EmployeeModel employee)
    {
        if (id!=employee.Id)
        {
            return BadRequest("Invalid employee ID.");
        }

        try
        {
            var (isValid, errorMessage) = await _employeeService.ValidateEmployeeAsync(employee);
            if (!isValid)
            {
                foreach (var error in errorMessage)
                {
                    ModelState.AddModelError("", error);
                }
            }

            if (ModelState.IsValid)
            {
                await _employeeService.UpdateEmployeeAsync(employee);
                TempData["SuccessMessage"] = "Employee updated successfully.";
                return RedirectToAction(nameof(Home));
            }
        }
        catch (Exception e)
        {
            ModelState.AddModelError("", "An error occurred while updating the employee: " + e.Message);
        }
        

        await PopulateDropdownAsync(employee.CompanyId);

        return View(employee);
    }

    /// <summary>
    /// Populates the dropdown list of companies for the employee form.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        if (id<=0)
        {
            return BadRequest();
        }

        try
        {
            var employee = await _employeeService.GetEmployeeByIdAsync(id);
            if (employee==null)
            {
                return NotFound("Employee not found.");
            }
            
            return View(employee);
        }
        catch (Exception e)
        {
            TempData["ErrorMessage"] = "An error occurred while loading the employee for deletion: " + e.Message;
            return RedirectToAction(nameof(Home));
        }
    }

    /// <summary>
    /// Handles the confirmation of employee deletion.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            var result = await _employeeService.DeleteEmployeeAsync(id);
            if (result)
            {
                TempData["SuccessMessage"] = "Employee deleted successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "An error occurred while deleting the employee.";
            }
        }
        catch (Exception e)
        {
            TempData["ErrorMessage"] = "An error occurred while deleting the employee: " + e.Message;
        }
        
        return RedirectToAction(nameof(Home));
    }
    
    /// <summary>
    /// Populates the dropdown list of legal forms for the company in the employee form.
    /// </summary>
    /// <param name="selectedValue"></param>
    private async Task PopulateDropdownAsync(int selectedValue = 0) // теперь non-nullable
    {
        var companies = await _companyService.GetAllCompaniesAsync() ?? new List<CompanyModel>();

        var selectList = companies
            .Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name,
                Selected = c.Id == selectedValue
            })
            .ToList();

        // Добавляем первый элемент
        selectList.Insert(0, new SelectListItem
        {
            Value = "0", // default "empty" option
            Text = "-- Выберите компанию --",
            Selected = selectedValue == 0
        });

        ViewBag.Companies = selectList;
    }






}