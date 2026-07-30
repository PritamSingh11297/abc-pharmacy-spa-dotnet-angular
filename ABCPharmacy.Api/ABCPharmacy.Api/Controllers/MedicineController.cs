using ABCPharmacy.Api.Models;
using ABCPharmacy.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace ABCPharmacy.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MedicineController : ControllerBase
{
    private readonly MedicineService _service;

    public MedicineController(MedicineService service)
    {
        _service = service;
    }

    [HttpGet]
    public IActionResult Get()
    {
        return Ok(_service.GetMedicines());
    }

    [HttpGet("search")]
    public IActionResult Search(string name)
    {
        var result = _service.GetMedicines().Where(x =>
            (!string.IsNullOrEmpty(x.FullName) &&
             x.FullName.Contains(name, StringComparison.OrdinalIgnoreCase))
            ||
            (!string.IsNullOrEmpty(x.Brand) &&
             x.Brand.Contains(name, StringComparison.OrdinalIgnoreCase))
        );

        return Ok(result);
    }

    [HttpPost]
    public IActionResult Add(Medicine medicine)
    {
        if (string.IsNullOrWhiteSpace(medicine.FullName))
            return BadRequest("Medicine Name is required");

        if (string.IsNullOrWhiteSpace(medicine.Brand))
            return BadRequest("Brand is required");

        if (medicine.Quantity <= 0)
            return BadRequest("Quantity must be greater than 0");

        if (medicine.Price <= 0)
            return BadRequest("Price must be greater than 0");

        if (medicine.ExpiryDate <= DateTime.Today)
            return BadRequest("Expiry Date must be a future date");

        var medicines = _service.GetMedicines();

        medicine.Id = medicines.Count == 0
            ? 1
            : medicines.Max(x => x.Id) + 1;

        medicines.Add(medicine);

        _service.SaveMedicines(medicines);

        return Ok(medicine);
    }

    [HttpPost("sale")]
    public IActionResult Sell(Sale sale)
    {
        var medicines = _service.GetMedicines();

        var medicine = medicines.FirstOrDefault(x => x.Id == sale.MedicineId);

        if (medicine == null)
            return BadRequest("Medicine not found");

        if (medicine.Quantity < sale.QuantitySold)
            return BadRequest("Not enough stock");

        medicine.Quantity -= sale.QuantitySold;

        _service.SaveMedicines(medicines);

        var sales = _service.GetSales();

        sale.Id = sales.Count == 0 ? 1 : sales.Max(x => x.Id) + 1;

        sale.SaleDate = DateTime.Now;

        sales.Add(sale);

        _service.SaveSales(sales);

        return Ok(sale);
    }

    [HttpGet("sales")]
    public IActionResult Sales()
    {
        return Ok(_service.GetSales());
    }
}