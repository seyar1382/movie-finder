// using RestSharp;
using System.ComponentModel;
using MauiTest.Src.Pages;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Layouts;
using TMDbLib.Client;


namespace MauiTest;

public partial class MainPage : ContentPage
{
	private TMDbClient client;

	public MainPage()
	{
		InitializeComponent();

		client = new TMDbClient("96b0f176afcf825ea97b650615c47a02");

		motvLogo.Source = Application.Current.RequestedTheme == AppTheme.Dark ? "motv.png" : "logo_light.svg";
	}

	private async void OnSearchClicked(object? sender, EventArgs e)
	{
		movieResults.Children.Clear();
		
    	string query = searchBar.Text;

    	if (string.IsNullOrWhiteSpace(query))
    		return;

    	try
    	{

				if (searchType == "Movies")
				{
					
				}
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
					// cardCreation(show);
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

	private string searchType = "Movies";
	
	private void OnShowsTabClicked(object sender, EventArgs e)
	{
		searchType = "TV Shows";
		showTab.BackgroundColor = Colors.Gray;
		movieTab.BackgroundColor = Colors.Black;
	}

	private void OnMoviesTabClicked(object sender, EventArgs e)
	{
		searchType = "TV Shows";
		showTab.BackgroundColor = Colors.Black;
		movieTab.BackgroundColor = Colors.Gray;
	}


	// function used in the creation of creating the movie cards 
	internal async void cardCreation(TMDbLib.Objects.Search.SearchMovie movie)
	{

		Border informationCard = new Border();
		informationCard.Style = (Style)Application.Current.Resources["InformationCard"];

		// sets the size the information cards, and taking the available space
		FlexLayout.SetBasis(informationCard, 600);
		FlexLayout.SetGrow(informationCard, 1);

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
			LineBreakMode = LineBreakMode.WordWrap,
			HorizontalOptions = LayoutOptions.Fill
		};

		Border imageBorder = new Border();
		imageBorder.Style = (Style)Application.Current.Resources["ImageBorder"];

		Image thumbnail = new Image()
		{
			// if the poster path is null, it will use a default image.
			Source = string.IsNullOrEmpty(movie.PosterPath) ? "placeholder_image.jpeg" : $"https://image.tmdb.org/t/p/w500{movie.PosterPath}",
			WidthRequest = 200,
			Aspect = Aspect.Fill
		};

		imageBorder.Content = thumbnail;

		//creates a vertical stack
		VerticalStackLayout movieDetails = new VerticalStackLayout();
		movieDetails.MaximumWidthRequest = 360; // do not touch this
		movieDetails.HorizontalOptions = LayoutOptions.Fill;

		//adds children to the verticall stack
		movieDetails.Children.Add(title);
		movieDetails.Children.Add(rating);
		movieDetails.Children.Add(overview);

		//initialize a Grid layout
		Grid content = new Grid
		{
			ColumnDefinitions =
			{
				new ColumnDefinition {Width = 220},
				new ColumnDefinition {Width = GridLength.Star}
			}
		};

		// applying hover effect. the grid changes colour when the pointer hovers over it.
		PointerGestureRecognizer pointer = new PointerGestureRecognizer();
		pointer.PointerEntered += (sender, e) =>
		{
			// content.BackgroundColor = Color.FromArgb("#FF35213E");
			content.BackgroundColor = 
				Application.Current.RequestedTheme == AppTheme.Dark 
				? (Color)Application.Current.Resources["HoverDark"] 
				: (Color)Application.Current.Resources["HoverLight"];
		};

		pointer.PointerExited += (sender, e) =>
		{
			// content.BackgroundColor = Color.FromArgb("#FF27192E");
			content.BackgroundColor =
				Application.Current.RequestedTheme == AppTheme.Dark 
				? (Color)Application.Current.Resources["Gray600"] 
				: (Color)Application.Current.Resources["CardLight"];
		};

		content.GestureRecognizers.Add(pointer);

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

		//adds the thumbnail to the grid with details next to it
		content.Add(imageBorder, 0, 0);
		content.Add(movieDetails, 1, 0);
				
		informationCard.Content = content;

		//adds the information card on the movieresults flexlayout
		movieResults.Children.Add(informationCard);
	}

}
