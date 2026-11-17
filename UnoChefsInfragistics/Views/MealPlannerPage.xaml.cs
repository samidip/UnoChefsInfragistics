using Infragistics.Controls.Charts;
using Infragistics.Controls.Gauges;
using BrushCollection = Infragistics.Controls.BrushCollection;

namespace Chefs.Views;

public sealed partial class MealPlannerPage : Page
{
	public MealPlannerPage()
	{
		this.InitializeComponent();
	}

	// One color per meal series (Breakfast, Lunch, Dinner, Snack), taken from the app palette
	private void OnCaloriesChartLoaded(object sender, RoutedEventArgs e)
	{
		if (sender is not XamCategoryChart chart)
		{
			return;
		}

		var brushes = new BrushCollection();
		foreach (var key in new[] { "NutritionProteinValBrush", "NutritionCarbsValBrush", "PrimaryBrush", "SecondaryBrush" })
		{
			brushes.Add(GetBrush(key));
		}

		chart.Brushes = brushes;
		chart.Outlines = brushes;
	}

	// Green band around the daily goal, amber above it
	private void OnCalorieGaugeLoaded(object sender, RoutedEventArgs e)
	{
		if (sender is not XamRadialGauge gauge || gauge.Ranges.Count > 0)
		{
			return;
		}

		var goal = MealPlannerModel.DailyCalorieGoal;
		gauge.Ranges.Add(new RadialGaugeRange { StartValue = goal * 0.85, EndValue = goal * 1.1, Brush = new SolidColorBrush(Microsoft.UI.ColorHelper.FromArgb(0xFF, 0x4C, 0xAF, 0x50)) });
		gauge.Ranges.Add(new RadialGaugeRange { StartValue = goal * 1.1, EndValue = gauge.MaximumValue, Brush = new SolidColorBrush(Microsoft.UI.ColorHelper.FromArgb(0xFF, 0xFF, 0xB3, 0x00)) });
	}

	private static SolidColorBrush GetBrush(string key)
		=> Application.Current.Resources.TryGetValue(key, out var value) && value is SolidColorBrush brush
			? brush
			: new SolidColorBrush(Microsoft.UI.Colors.Gray);
}
