// using RestSharp;
using System.ComponentModel;
using Microsoft.Maui.Controls.Shapes;
using TMDbLib.Client;
using TMDbLib.Objects.General;

namespace MauiTest;

public partial class MainPage : ContentPage
{
	private TMDbClient client;

	public MainPage()
	{
		InitializeComponent();
		client = new TMDbClient("96b0f176afcf825ea97b650615c47a02");
		// async Task GetMovie()
		// {
		// 	var options = new RestClientOptions("https://api.themoviedb.org/3/authentication");
		// 	var client = new RestClient(options);
		// 	var request = new RestRequest("");
		// 	request.AddHeader("accept", "application/json");
		// 	request.AddHeader("Authorization", "Bearer ••••");
		// 	var response = await client.GetAsync(request);

		// 	Console.WriteLine("{0}", response.Content);
		// }
	}

	// async Task Wrapper()
	// {
	// 	var movie = await client.GetMovieAsync(47964);
	// 	Console.WriteLine($"Movie name: {movie?.Title}");
	// }

	// private async void OnSearchButtonClicked(object? sender, EventArgs e)
	// {
	// 	await Wrapper();
	// }

	private async void OnSearchClicked(object sender, EventArgs e)
	{
		movieResults.Children.Clear();

    string query = searchBar.Text;

    if (string.IsNullOrWhiteSpace(query))
      return;

    try
    {
			

			var movies = await client.SearchMovieAsync(query);

			foreach (var movie in movies?.Results ?? [])
			{

				
				Border card = new Border()
				{
					StrokeThickness = 0,
					StrokeShape = new RoundRectangle()
					{
						CornerRadius = 10
					},
					Margin = 10,
					Background = Brush.Grey,
					Padding = 10
				};
				
				Label title = new Label()
				{
					Text = movie.Title,
					FontSize = 24,
					
				};
				Label rating = new Label()
				{
					Text = movie.VoteAverage.ToString()
				};
				Label overview = new Label()
				{
					Text = movie.Overview,
					WidthRequest = 900
				};
				Border imageBorder = new Border()
				{
					StrokeThickness = 0,
					StrokeShape = new RoundRectangle()
					{
						CornerRadius = 10
					},
					Margin = new Thickness(0, 0, 15, 0)
				};

				Image thumbnail = new Image()
				{
					Source = $"https://image.tmdb.org/t/p/w500{movie.PosterPath}",
					WidthRequest = 200,
				};

				imageBorder.Content = thumbnail;

				VerticalStackLayout movieDetails = new VerticalStackLayout();


				movieDetails.Children.Add(title);
				movieDetails.Children.Add(rating);
				movieDetails.Children.Add(overview);

				HorizontalStackLayout horizontal = new HorizontalStackLayout();

				horizontal.Children.Add(imageBorder);
				horizontal.Children.Add(movieDetails);
				
				card.Content = horizontal;

				movieResults.Children.Add(card);

				var providers = await client.GetMovieWatchProvidersAsync(movie.Id);
				var ukProviders = providers?.Results?["GB"];

				foreach (var provider in ukProviders?.Buy ?? [])
				{
					Label movieProvider = new Label()
					{
						Text = $"Streaming on: {provider.ProviderName}"
					};
					movieDetails.Children.Add(movieProvider);
				}

			}

			

			var shows = await client.SearchTvShowAsync(query);

			foreach (var show in shows?.Results ?? [])
			{
				Console.WriteLine($"TV SHOW: {show.Name}");
				Console.WriteLine($"Overview: {show.Overview}");
				Console.WriteLine($"Rating: {show.VoteAverage}");
				Console.WriteLine($"Origianl Language: {show.OriginalLanguage}");
				Console.WriteLine($"Popularity: {show.PosterPath}");
			}		
    }
    catch (Exception ex)
    {
      Console.WriteLine($"ERROR: {ex.Message}");
    }
	}
}
