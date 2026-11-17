namespace Chefs.Business.Models;

public partial record NutritionChartItem(string Name, double Value, double MaxValueRef)
{
	public string Label => $"{Name} {Value}/{MaxValueRef}g";
}
