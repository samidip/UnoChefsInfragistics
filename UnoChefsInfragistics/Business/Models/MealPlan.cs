namespace Chefs.Business.Models;

public partial record PlannedMeal(
	DateTime Date,
	string DayLabel,
	string Meal,
	string? Recipe,
	string? Category,
	string? ImageUrl,
	int CookMinutes,
	int Calories)
{
	// XamDataGrid's ImageColumn doesn't resolve ms-appx:/// URIs, but loads file:// URIs as a remote resource.
	// This works for desktop targets where assets sit next to the app binaries.
	public string? ImageFileUri => ImageUrl is null
		? null
		: new Uri(Path.Combine(AppContext.BaseDirectory, ImageUrl.Replace("ms-appx:///", string.Empty))).AbsoluteUri;
}

public partial record DailyCalories(
	string Day,
	double Breakfast,
	double Lunch,
	double Dinner,
	double Snack);

public partial record MealPlan(
	IImmutableList<PlannedMeal> Meals,
	IImmutableList<DailyCalories> Daily,
	double DailyGoal)
{
	private static readonly string[] MealSlots = ["Breakfast", "Lunch", "Dinner", "Snack"];

	public double AverageDailyCalories => Daily.Count == 0 ? 0 : Math.Round(Daily.Average(d => d.Breakfast + d.Lunch + d.Dinner + d.Snack));

	public int TotalCookHours => (int)Math.Round(Meals.Sum(m => m.CookMinutes) / 60.0);

	// Rotates each category's recipes across the week so the plan is stable for a given start date
	public static MealPlan Create(IImmutableList<Recipe> recipes, DateTime weekStart, double dailyGoal)
	{
		var byCategory = MealSlots.ToDictionary(
			slot => slot,
			slot => recipes.Where(r => r.Category.Name == slot).ToList());

		var meals = new List<PlannedMeal>();
		var daily = new List<DailyCalories>();

		for (var day = 0; day < 7; day++)
		{
			var date = weekStart.AddDays(day);
			var calories = new Dictionary<string, int>();

			foreach (var slot in MealSlots)
			{
				var options = byCategory[slot];
				if (options.Count == 0)
				{
					continue;
				}

				var recipe = options[day % options.Count];
				var kcal = ParseCalories(recipe.Calories);
				calories[slot] = kcal;

				meals.Add(new PlannedMeal(
					date,
					// "Day N" prefix keeps groups in chronological order when the grid sorts them
					$"Day {day + 1} · {date:dddd, MMM d}",
					slot,
					recipe.Name,
					recipe.Category.Name,
					recipe.ImageUrl,
					(int)Math.Round(new TimeSpan(recipe.CookTime?.Ticks ?? 0).TotalMinutes),
					kcal));
			}

			daily.Add(new DailyCalories(
				$"{date:ddd}",
				calories.GetValueOrDefault("Breakfast"),
				calories.GetValueOrDefault("Lunch"),
				calories.GetValueOrDefault("Dinner"),
				calories.GetValueOrDefault("Snack")));
		}

		return new MealPlan(meals.ToImmutableList(), daily.ToImmutableList(), dailyGoal);
	}

	private static int ParseCalories(string? calories)
		=> int.TryParse(new string((calories ?? string.Empty).TakeWhile(char.IsDigit).ToArray()), out var kcal) ? kcal : 0;
}
