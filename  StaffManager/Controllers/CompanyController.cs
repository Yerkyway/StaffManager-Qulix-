using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using StaffManager.Models;
using StaffManager.Services;

namespace StaffManager.Controllers;

/// <summary>
/// Controller for managing companies in the Staff Manager application.
/// </summary>
public class CompanyController : Controller
{
    private readonly CompanyService _companyService;
    
    public CompanyController(CompanyService companyService)
    {
        _companyService = companyService;
    }

    /// <summary>
    /// Displays a list of all companies.
    /// </summary>
    /// <returns></returns>
    public async Task<IActionResult> Home()
    {
        try
        {
            var companies = await _companyService.GetAllCompaniesAsync();
            return View(companies);
        }
        catch (Exception e)
        {
            TempData["ErrorMessage"] = "An error occurred while loading the companies: " + e.Message;
            return View(new List<CompanyModel>());
        }
    }

    /// <summary>
    /// Displays the details of a specific company by its ID.
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
            var company = await _companyService.GetCompanyByIdAsync(id);
            if (company == null)
            {
                return NotFound("Company not found.");
            }

            return View(company);
        }
        catch (Exception e)
        {
            TempData["ErrorMessage"] = "An error occurred while loading the company: " + e.Message;
            return RedirectToAction(nameof(Home));
        }
    }

    /// <summary>
    /// Displays the form for creating a new company.
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        PopulateLegalFormsOfCompany();
        return View(new CompanyModel());
    }

    /// <summary>
    /// Handles the submission of the form to create a new company.
    /// </summary>
    /// <param name="company"></param>
    /// <returns></returns>
    [HttpPost, ActionName("Create")]
    public async Task<IActionResult> Create(CompanyModel company)
    {
        try
        {
            var (isValid, errorMessage) = await _companyService.ValidateCompanyAsync(company);
            if (!isValid)
            {
                foreach (var error in errorMessage)
                {
                    ModelState.AddModelError(" ", error);
                }
            }

            if (ModelState.IsValid)
            {
                var companyId = await _companyService.CreateCompanyAsync(company);
                TempData["SuccessMessage"] = $"Компания '{company.Name}' успешно создана";
                return RedirectToAction(nameof(Home));
            }
        }
        catch (Exception e)
        {
            ModelState.AddModelError("", "An error occurred while creating the company. Please try again later.");
        }
        
        PopulateLegalFormsOfCompany();
        return View(company);
    }

    /// <summary>
    /// Populates the legal form of the company dropdown list.
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
            var company = await _companyService.GetCompanyByIdAsync(id);
            if (company == null)
            {
                return NotFound("Company not found.");
            }
            
            PopulateLegalFormsOfCompany(company.LegalForm);
            return View(company);
        }
        catch (Exception e)
        {
            TempData["ErrorMessage"] = "An error occured while loading the company: " + e.Message;
            return RedirectToAction(nameof(Home));
        }
    }

    /// <summary>
    /// Handles the submission of the form to update an existing company.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="company"></param>
    /// <returns></returns>
    [HttpPost, ActionName("Update")]
    public async Task<IActionResult> Update(int id, CompanyModel company)
    {
        if (id!=company.Id)
        {
            return BadRequest();
        }

        try
        {
            var (isValid, errorsMessage) = await _companyService.ValidateCompanyAsync(company);
            if (!isValid)
            {
                foreach (var error in errorsMessage)
                {
                    ModelState.AddModelError("", error);
                }
            }

            if (ModelState.IsValid)
            {
                await _companyService.UpdateCompanyAsync(company);
                TempData["SuccessMessage"] = $"Компания '{company.Name}' успешно обновлена";
                return RedirectToAction(nameof(Home));
            }
        }
        catch (Exception e)
        {
            ModelState.AddModelError("", "An error occurred while updating the company. Please try again later.");
        }
        
        PopulateLegalFormsOfCompany(company.LegalForm);
        return View(company);
    }

    /// <summary>
    /// Displays the confirmation page for deleting a company by its ID.
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
            var company = await _companyService.GetCompanyByIdAsync(id);
            if (company == null)
            {
                return NotFound("Company not found.");
            }
            
            return View(company);
        }
        catch (Exception e)
        {
            TempData["ErrorMessage"] = "An error occurred while loading the company: " + e.Message;
            return RedirectToAction(nameof(Home));
        }
    }
    
    /// <summary>
    /// Handles the submission of the form to delete a company by its ID.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            var results = await _companyService.DeleteCompanyAsync(id);
            if (results)
            {
                TempData["SuccessMessage"] = "Компания успешно удалена";
            }
            else
            {
                TempData["ErrorMessage"] = "Не удалось удалить компанию. Возможно, она содержит сотрудников.";
            }
        }
        catch (Exception e)
        {
            TempData["ErrorMessage"] = "An error occurred while deleting the company: " + e.Message;
        }
        
        return RedirectToAction(nameof(Home));
    }

    /// <summary>
    /// Populates the legal form of the company dropdown list for the create and update views.
    /// </summary>
    /// <param name="selectedLegalForm"></param>
    private void PopulateLegalFormsOfCompany(string? selectedLegalForm = null)
    {
        var legalForms = new List<string>
        {
            "ООО", "ЗАО", "ОАО", "ИП", "АО", "ПАО", "НКО", "ГУП", "МУП"
        };
        ViewBag.LegalForms = new SelectList(legalForms, selectedLegalForm);
    }
}