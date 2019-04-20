using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows;

namespace EEArtify {
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window {

		private string allBlocksImagePath = "";
		private string inputImagePath = "";
		private string outputImagePath = "";

		Converter converter = new Converter();

		//Objects for the dropdown menu to select the algorithm. The selected algorithms delegate will then be passed to the 
		private List<ComboBoxElement> algs = new List<ComboBoxElement> {
			new ComboBoxElement(ComparisonAlgorithms.DeltaE76, "deltaE76"),
			new ComboBoxElement(ComparisonAlgorithms.DeltaE94, "deltaE94"),
			new ComboBoxElement(ComparisonAlgorithms.CMCcl, "CMCcl"),
			new ComboBoxElement(ComparisonAlgorithms.DeltaE2000, "deltaE2000")
		};

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
				allBlocksImagePath = ofd.FileName;
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
			if (allBlocksImagePath == "" || inputImagePath == "" || outputImagePath == "") {
				MessageBox.Show("Please select all image paths!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}

			Bitmap image;
			try {
				image = new Bitmap(inputImagePath);
			} catch {
				MessageBox.Show("Input image not found!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}

			Bitmap allBlocksImage;
			try {
				allBlocksImage = new Bitmap(allBlocksImagePath);
			} catch {
				MessageBox.Show("Input image not found!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}

			var allowedColors = new LinkedList<Color>();

			//puts all colors from the AllBlocks filter into a LinkedList
			for (int y = 0; y < allBlocksImage.Height; y++) {
				for (int x = 0; x < allBlocksImage.Width; x++) {
					var color = allBlocksImage.GetPixel(x, y);
					if (!allowedColors.Contains(color))
						allowedColors.AddLast(color);
				}
			}

			converter.AddUpdater(ConversionProgress_ProgressBar.Dispatcher, UpdateProgressBar);
			
			var stopwatch = new Stopwatch();
			stopwatch.Start();
			await converter.StartAsync(allowedColors, image, algs[Algorithm_ComboBox.SelectedIndex].Algorithm);
			stopwatch.Stop();

			converter.Image.Save(outputImagePath);
			ConversionProgress_ProgressBar.Value = 1.0;
			ConversionProgress_TextBox.Text = $"Done! ({ stopwatch.ElapsedMilliseconds } ms)";
			MessageBox.Show($"Conversion finished! Took { stopwatch.ElapsedMilliseconds } ms", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
		}
		
		private void UpdateProgressBar(int progress) {
			ConversionProgress_ProgressBar.Value = (double) progress / converter.TotalWork;
			ConversionProgress_TextBox.Text = $"Working... { progress }/{ converter.TotalWork }";
		}
	}
}
