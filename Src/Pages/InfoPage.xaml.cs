namespace MauiTest.Src.Pages;

public partial class InfoPage : ContentPage
{
	public InfoPage(string movieName)
	{
		InitializeComponent();
	}

	private void BackClick(object? sender, EventArgs e)
	{
		Navigation.PopModalAsync();
	}

	private void InfoPageBuilderMovie()
	{
		
	}

}