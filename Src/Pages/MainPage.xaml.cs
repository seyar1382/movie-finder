// using RestSharp;
using System.ComponentModel;
using MauiTest.Src.Pages;
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

	}

	private async void OnSearchClicked(object sender, EventArgs e)
	{
		movieResults.Children.Clear();
		
    	string query = searchBar.Text;

    	if (string.IsNullOrWhiteSpace(query))
    		return;

    	try
    	{
			// Search for movies using the TMDb API
			var movies = await client.SearchMovieAsync(query);

			foreach (var movie in movies?.Results ?? [])
			{

				cardCreation(movie);
		
			}
			
			//code for tv shows for debug purposes
			// search for TV shows using the TMDb API
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

	// going to be used to make a button?
	internal void buttonTemplate()
	{
		
	}

	// function used internally to load info page
	internal void infoPageLoad(string movieName)
	{
		Navigation.PushModalAsync(new InfoPage(movieName));
	}


	// function used in the creation of creating the movie cards 
	internal async void cardCreation(TMDbLib.Objects.Search.SearchMovie movie)
	{

		Console.WriteLine("card creation");

		Border informationCard = new Border()
		{
			// StrokeThickness = 0,
			// StrokeShape = new RoundRectangle()
			// {
			// 	CornerRadius = 10
			// },
			// Margin = 10,
			// Background = Brush.Grey,
			// Padding = 10,
			Style = (Style)Application.Current.Resources["InformationCard"]
		};


		//defines all the text parts
		Label title = new Label()
		{
			Text = movie.Title,
			FontSize = 24
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
			// if the poster path is null, it will use a default image, currently not working?
			Source = $"https://image.tmdb.org/t/p/w500{movie.PosterPath}" ?? "../Resources/Images/[Deleted image].jpeg",
			WidthRequest = 200
		};

		imageBorder.Content = thumbnail;

		//creates a vertical stack
		VerticalStackLayout movieDetails = new VerticalStackLayout();

		//adds children to the verticall stack
		movieDetails.Children.Add(title);
		movieDetails.Children.Add(rating);
		movieDetails.Children.Add(overview);

		//initialize a horizontal stack layout
		HorizontalStackLayout horizontal = new HorizontalStackLayout();

		try{
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
		catch (Exception ex)
		{
			Console.WriteLine($"Error finding the service: {ex.Message}");
		}

		//adds the thumbnail to the stack with description and title next to it
		horizontal.Children.Add(imageBorder);
		horizontal.Children.Add(movieDetails);
				
		informationCard.Content = horizontal;

		//adds the information card on the moviresults verticall stack layout
		movieResults.Children.Add(informationCard);
	}

}
