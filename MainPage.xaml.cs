// using RestSharp;
using System.ComponentModel;
using Microsoft.Maui.Controls.Shapes;
using TMDbLib.Client;

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

    string query = searchBar.Text;

    if (string.IsNullOrWhiteSpace(query))
      return;

    try
    {
			

			var movies = await client.SearchMovieAsync(query);

			foreach (var movie in movies?.Results ?? [])
			{
				
				Border card = new Border();
				card.StrokeThickness = 0;
				card.StrokeShape = new RoundRectangle()
				{
					CornerRadius = 10
				};
				card.Margin = 10;


				Label title = new Label()
				{
					Text = movie.Title
				};
				Label adult = new Label()
				{
					Text = movie.Adult.ToString()
				};
				Label overview = new Label()
				{
					Text = movie.Overview
				};
				Image thumbnail = new Image()
				{
					Source = $"https://image.tmdb.org/t/p/w500{movie.PosterPath}",
					WidthRequest = 100
				};
				VerticalStackLayout movieDetails = new VerticalStackLayout();


				movieDetails.Children.Add(title);
				movieDetails.Children.Add(adult);
				movieDetails.Children.Add(overview);

				HorizontalStackLayout horizontal = new HorizontalStackLayout();

				horizontal.Children.Add(thumbnail);
				horizontal.Children.Add(movieDetails);
				
				card.Content = horizontal;

				movieResults.Children.Add(card);
				Console.WriteLine($"Title: {movie.Title}");
				Console.WriteLine($"Rating: {movie.VoteAverage}");
				Console.WriteLine($"Overview: {movie.Overview}");
				Console.WriteLine($"Release Date: {movie.ReleaseDate}");
				Console.WriteLine($"Popularity: {movie.Popularity}");
				Console.WriteLine($"Popularity: {movie.PosterPath}");
				Console.WriteLine("-------------------");

				
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
