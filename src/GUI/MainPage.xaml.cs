using RestSharp;
using TMDbLib.Client;

namespace MauiTest;


public partial class MainPage : ContentPage
{

	public MainPage()
	{
		InitializeComponent();
		
		async Task GetMovie()
		{
			var options = new RestClientOptions("https://api.themoviedb.org/3/authentication");
			var client = new RestClient(options);
			var request = new RestRequest("");
			request.AddHeader("accept", "application/json");
			request.AddHeader("Authorization", "Bearer ••••");
			var response = await client.GetAsync(request);

			Console.WriteLine("{0}", response.Content);
		}

		async Task Wrapper()
		{
			var client = new TMDbClient("96b0f176afcf825ea97b650615c47a02");
			var movie = await client.GetMovieAsync(47964);

			Console.WriteLine($"Movie name: {movie.Title}");
		}

		

		
		
		}



	
}
