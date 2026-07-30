using System.Text.Json;
using ABCPharmacy.Api.Models;

namespace ABCPharmacy.Api.Services;

public class MedicineService
{
    private readonly string medicineFile = "Data/medicines.json";
    private readonly string salesFile = "Data/sales.json";

    public List<Medicine> GetMedicines()
    {
        if (!File.Exists(medicineFile))
            return new List<Medicine>();

        var json = File.ReadAllText(medicineFile);

        if (string.IsNullOrWhiteSpace(json))
            return new List<Medicine>();

        return JsonSerializer.Deserialize<List<Medicine>>(json) ?? new List<Medicine>();
    }

    public void SaveMedicines(List<Medicine> medicines)
    {
        var json = JsonSerializer.Serialize(medicines, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        File.WriteAllText(medicineFile, json);
    }

    public List<Sale> GetSales()
    {
        if (!File.Exists(salesFile))
            return new List<Sale>();

        var json = File.ReadAllText(salesFile);

        if (string.IsNullOrWhiteSpace(json))
            return new List<Sale>();

        return JsonSerializer.Deserialize<List<Sale>>(json) ?? new List<Sale>();
    }

    public void SaveSales(List<Sale> sales)
    {
        var json = JsonSerializer.Serialize(sales, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        File.WriteAllText(salesFile, json);
    }
}