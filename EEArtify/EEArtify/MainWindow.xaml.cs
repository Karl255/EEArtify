using ColorMine.ColorSpaces.Comparisons;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows;

namespace EEArtify {
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window {

		//Objects for the dropdown menu to select the algorithm. The selected algorithms delegate will then be passed to the 
		private List<ComboBoxElement> algs = new List<ComboBoxElement> {
			new ComboBoxElement(ComparisonAlgorithms.DeltaE76, "deltaE76"),
			new ComboBoxElement(ComparisonAlgorithms.DeltaE94, "deltaE94"),
			new ComboBoxElement(ComparisonAlgorithms.CMCcl, "CMCcl"),
			new ComboBoxElement(ComparisonAlgorithms.DeltaE2000, "deltaE2000")
		};

		private string allBlocksImage = "";
		private string inputImagePath = "";
		private string outputImagePath = "";

		public MainWindow() {
			InitializeComponent();
			Algorithm_ComboBox.ItemsSource = algs;
			Algorithm_ComboBox.SelectedIndex = 0;
		}

		private void AllBlocksImage_Button_Click(object sender, RoutedEventArgs e) {
			var ofd = new OpenFileDialog {
				Filter = "PNG Image (*.png)|*.png",
				InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
			};
			if (ofd.ShowDialog() == true) {
				allBlocksImage = ofd.FileName;
				AllBlocksImage_TextBox.Text = ofd.FileName;
			}
		}

		private void InputImage_Button_Click(object sender, RoutedEventArgs e) {
			var ofd = new OpenFileDialog {
				Filter = "PNG Image (*.png)|*.png",
				InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
			};
			if (ofd.ShowDialog() == true) {
				inputImagePath = ofd.FileName;
				InputImage_TextBox.Text = ofd.FileName;
			}
		}

		private void Output_Button_Click(object sender, RoutedEventArgs e) {
			var sfd = new SaveFileDialog {
				Filter = "PNG Image (*.png)|*.png",
				InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
			};
			if (sfd.ShowDialog() == true) {
				outputImagePath = sfd.FileName;
				OutputImage_TextBox.Text = sfd.FileName;
			}
		}

		private async void Start_Button_Click(object sender, RoutedEventArgs e) {
			if (allBlocksImage == "" || inputImagePath == "" || outputImagePath == "") {
				MessageBox.Show("Please select all image paths!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}

			Bitmap image;
			try {
				image = new Bitmap(inputImagePath);
			} catch {
				MessageBox.Show("Input image not found!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}

			var allowedColors = new List<Color>();

			for(int y = 0; y < image.Height; y++) {
				for(int x = 0; x < image.Width; x++) {
					allowedColors.Add(image.GetPixel(x, y));
				}
			}
			
			var converter = new Converter();
			
			//converter.AddUpdater(ConversionProgress_ProgressBar.Dispatcher, UpdateProgressBar);
			await converter.Start(allowedColors, image, algs[Algorithm_ComboBox.SelectedIndex].Algorithm);
			converter.Image.Save(outputImagePath);

			MessageBox.Show("Conversion finished!", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
		}

		/*
		private void UpdateProgressBar() {
			ConversionProgress_ProgressBar.Value = converter.Progress / converter.TotalWork;
			ConversionProgress_TextBox.Text = $"{ (converter.Progress != converter.TotalWork ? "Working..." : "Done!") } { converter.Progress }/{ converter.TotalWork }";
		}
		*/
	}
}
