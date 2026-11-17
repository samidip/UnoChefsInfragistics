using BrushCollection = Infragistics.Controls.BrushCollection;
using Infragistics.Controls.Gauges;
using Microsoft.UI;

namespace Chefs.Views.Controls;
public sealed partial class ChartControl : UserControl
{
	private Recipe? _recipe;


	public SolidColorBrush CarbBrush
	{
		get { return (SolidColorBrush)GetValue(CarbBrushProperty); }
		set { SetValue(CarbBrushProperty, value); }
	}

	public static readonly DependencyProperty CarbBrushProperty =
		DependencyProperty.Register("CarbBrush", typeof(SolidColorBrush), typeof(ChartControl), new PropertyMetadata(new SolidColorBrush(Colors.Black), OnBrushChanged));

	public SolidColorBrush ProteinBrush
	{
		get { return (SolidColorBrush)GetValue(ProteinBrushProperty); }
		set { SetValue(ProteinBrushProperty, value); }
	}

	public static readonly DependencyProperty ProteinBrushProperty =
		DependencyProperty.Register("ProteinBrush", typeof(SolidColorBrush), typeof(ChartControl), new PropertyMetadata(new SolidColorBrush(Colors.Black), OnBrushChanged));

	public SolidColorBrush FatBrush
	{
		get { return (SolidColorBrush)GetValue(FatBrushProperty); }
		set { SetValue(FatBrushProperty, value); }
	}

	public static readonly DependencyProperty FatBrushProperty =
		DependencyProperty.Register("FatBrush", typeof(SolidColorBrush), typeof(ChartControl), new PropertyMetadata(new SolidColorBrush(Colors.Black), OnBrushChanged));



	public SolidColorBrush DataLabelBrush
	{
		get { return (SolidColorBrush)GetValue(DataLabelBrushProperty); }
		set { SetValue(DataLabelBrushProperty, value); }
	}

	public static readonly DependencyProperty DataLabelBrushProperty =
		DependencyProperty.Register("DataLabelBrush", typeof(SolidColorBrush), typeof(ChartControl), new PropertyMetadata(new SolidColorBrush(Colors.Black), OnBrushChanged));



	public SolidColorBrush TrackBackgroundBrush
	{
		get { return (SolidColorBrush)GetValue(TrackBackgroundBrushProperty); }
		set { SetValue(TrackBackgroundBrushProperty, value); }
	}

	public static readonly DependencyProperty TrackBackgroundBrushProperty =
		DependencyProperty.Register("TrackBackgroundBrush", typeof(SolidColorBrush), typeof(ChartControl), new PropertyMetadata(new SolidColorBrush(Colors.Black), OnBrushChanged));

	private static void OnBrushChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
	{
		if (dependencyObject is not ChartControl chartControl) return;

		if (chartControl._recipe != null)
		{
			chartControl.BuildCharts();
		}
	}

	public ChartControl()
	{
		this.InitializeComponent();

		_recipe = DataContext as Recipe;
		if (_recipe != null)
		{
			BuildCharts();
		}

		DataContextChanged += OnDataContextChanged;
	}

	private void OnDataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
	{
		_recipe = args.NewValue as Recipe;

		if (_recipe != null)
		{
			BuildCharts();
		}
	}

	private void BuildCharts()
	{
		// Mock recipes carry no nutrition data, so fall back to the sample values the original chart used
		var nutrition = _recipe?.Nutrition;
		var chartData = new NutritionChartItem[]
		{
			new(nameof(Nutrition.Fat), nutrition?.Fat ?? 30, nutrition?.FatBase ?? 75),
			new(nameof(Nutrition.Carbs), nutrition?.Carbs ?? 101, nutrition?.CarbsBase ?? 300),
			new(nameof(Nutrition.Protein), nutrition?.Protein ?? 30, nutrition?.ProteinBase ?? 110),
		};

		BuildDoughnutChart(chartData);
		BuildBulletGraphs(chartData);
	}

	private void BuildDoughnutChart(NutritionChartItem[] chartData)
	{
		ringSeries.Brushes = new BrushCollection();
		foreach (var item in chartData)
		{
			ringSeries.Brushes.Add(GetNutritionBrush(item.Name));
		}
		ringSeries.ItemsSource = chartData;
	}

	private void BuildBulletGraphs(NutritionChartItem[] chartData)
	{
		bulletGraphs.Children.Clear();

		foreach (var item in chartData)
		{
			var bulletGraph = new XamBulletGraph
			{
				Height = 22,
				MinimumValue = 0,
				ScaleStartExtent = 0,
				ScaleEndExtent = 1,
				MaximumValue = item.MaxValueRef,
				Value = item.Value,
				TargetValue = item.MaxValueRef,
				TargetValueBreadth = 0,
				ValueBrush = GetNutritionBrush(item.Name),
				ValueInnerExtent = 0,
				ValueOuterExtent = 1,
				BackingBrush = new SolidColorBrush(Colors.Transparent),
				BackingOutline = new SolidColorBrush(Colors.Transparent),
				TickBrush = new SolidColorBrush(Colors.Transparent),
				MinorTickBrush = new SolidColorBrush(Colors.Transparent),
				LabelsVisible = false,
			};
			bulletGraph.Ranges.Add(new LinearGraphRange
			{
				StartValue = 0,
				EndValue = item.MaxValueRef,
				Brush = TrackBackgroundBrush,
				Outline = TrackBackgroundBrush,
			});

			bulletGraphs.Children.Add(new TextBlock
			{
				Text = item.Label,
				FontSize = 13,
				Foreground = DataLabelBrush,
			});
			bulletGraphs.Children.Add(bulletGraph);
		}
	}

	private SolidColorBrush GetNutritionBrush(string name)
	{
		return name switch
		{
			nameof(Nutrition.Carbs) => CarbBrush,
			nameof(Nutrition.Protein) => ProteinBrush,
			nameof(Nutrition.Fat) => FatBrush,
			_ => new SolidColorBrush(Colors.Yellow),
		};
	}
}
