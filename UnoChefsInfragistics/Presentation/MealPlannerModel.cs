using Chefs.Business.Services.Recipes;

namespace Chefs.Presentation;

public partial record MealPlannerModel(IRecipeService RecipeService)
{
	public const double DailyCalorieGoal = 1600;

	public IFeed<MealPlan> Plan => Feed.Async(async ct =>
	{
		var today = DateTime.Today;
		var weekStart = today.AddDays(-(((int)today.DayOfWeek + 6) % 7));
		return MealPlan.Create(await RecipeService.GetAll(ct), weekStart, DailyCalorieGoal);
	});
}
