using Infragistics.Controls;
using Infragistics.Controls.Charts;
using Infragistics.Controls.Maps;

namespace Chefs.Views;

public sealed partial class MapPage : Page
{
	private static List<Contributor> Contributors => new List<Contributor>
	{
		new Contributor("Troyan Smith", 45.5018, -73.5566, 45),
		new Contributor("Niki Samantha", 45.5411, -73.5770, 20),
		new Contributor("Mike Baker", 45.5113, -73.5961, 15)
	};

	private record Contributor(string? Name, double Lat, double Lng, int Recipes);

	private object? _calloutItem;

	public MapPage()
	{
		this.InitializeComponent();

		InitializeMap();
	}

	private void InitializeMap()
	{
		GeoMap.BackgroundContent = new OpenStreetMapImagery { UserAgent = "UnoChefsInfragistics" };

		var contributors = Contributors;
		ContributorSeries.ItemsSource = contributors;

		// TODO: Get real location
		var myLocation = new Contributor("My location", contributors.Average(c => c.Lat), contributors.Average(c => c.Lng), 0);
		MyLocationSeries.ItemsSource = new[] { myLocation };

		GeoMap.SeriesClick += OnSeriesClick;
		GeoMap.Loaded += (_, _) => CenterOn(myLocation, span: 0.1);
	}

	private void CenterOn(Contributor location, double span)
	{
		// Geographic rect: X/Width are longitude, Y/Height are latitude
		GeoMap.ZoomToGeographic(new IGRect(location.Lng - span / 2, location.Lat - span / 2, span, span));
	}

	private void OnSeriesClick(object sender, DataChartPointerEventArgs e)
	{
		if (e.Item is not Contributor contributor || ReferenceEquals(_calloutItem, contributor))
		{
			HideCallout();
			return;
		}

		_calloutItem = contributor;
		CalloutTitle.Text = contributor.Name;
		CalloutSubtitle.Text = contributor.Recipes > 0 ? $"{contributor.Recipes} recipes" : string.Empty;
		CalloutSubtitle.Visibility = contributor.Recipes > 0 ? Visibility.Visible : Visibility.Collapsed;

		Callout.Visibility = Visibility.Visible;
		Callout.UpdateLayout();
		Canvas.SetLeft(Callout, e.ChartPosition.X - Callout.ActualWidth / 2);
		Canvas.SetTop(Callout, e.ChartPosition.Y - Callout.ActualHeight - 16);
	}

	private void HideCallout()
	{
		_calloutItem = null;
		Callout.Visibility = Visibility.Collapsed;
	}

	private void Zoom(double factor)
	{
		HideCallout();

		// Scale the visible geographic area around its center
		var view = GeoMap.GetGeographicFromZoom(GeoMap.ActualWindowRect);
		var width = view.Width * factor;
		var height = view.Height * factor;
		GeoMap.ZoomToGeographic(new IGRect(view.Left + (view.Width - width) / 2, view.Top + (view.Height - height) / 2, width, height));
	}

	private void OnZoomInClick(object sender, RoutedEventArgs e)
	{
		Zoom(0.5);
	}

	private void OnZoomOutClick(object sender, RoutedEventArgs e)
	{
		Zoom(2);
	}
}
