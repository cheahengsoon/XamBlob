using System;
using System.Threading.Tasks;
using Plugin.Media;
using Xamarin.Forms;

namespace XamBlob
{
	public partial class XamBlobPage : ContentPage
	{
		public XamBlobPage()
		{
			InitializeComponent();
			this.Content = new ScrollView
			{ 
				Content = _wrapLayout
			};
		}

		WrapLayout _wrapLayout = new WrapLayout();
		bool _isRefreshing;

		async void HandleAddClicked(object sender, System.EventArgs e)
		{ 
			await CrossMedia.Current.Initialize();

			if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
			{
				DisplayAlert("No Camera", ":( No camera available.", "OK");
				return;
			}

			var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
			{
				Directory = "Sample",
				Name = "test.jpg"
			});

			if (file == null)
				return;

			Console.WriteLine("File: " + file.Path);

			await BlobMan.Instance.UploadFileAsync(file.Path);
		}

		async void HandleRefreshClicked(object sender, System.EventArgs e)
		{
			if (_isRefreshing)
			{
				return;
			}

			_isRefreshing = true;
			try
			{
				await UpdateAllImagesAsync();
			}
			finally
			{
				_isRefreshing = false;
			}
		}

		async Task UpdateAllImagesAsync()
		{
			var uris = await BlobMan.Instance.GetAllBlobUrisAsync();
			foreach (var uri in uris)
			{
				var img = new Image
				{
					Source = ImageSource.FromUri(uri),
					WidthRequest = 70
				};

				_wrapLayout.Children.Add(img);
			}
		}
	}
}
