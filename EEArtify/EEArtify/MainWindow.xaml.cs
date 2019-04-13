using ColorMine.ColorSpaces.Comparisons;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Windows;

namespace EEArtify {
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window {

		//Objects for the dropdown menu to select the algorithm. The selected algorithms delegate will then be passed to the 
		List<ComboBoxElement> algs = new List<ComboBoxElement> {
			new ComboBoxElement(new Cie1976Comparison().Compare, "deltaE76"),
			new ComboBoxElement(new Cie94Comparison().Compare, "deltaE94"),
			new ComboBoxElement(new CmcComparison().Compare, "CMCcl"),
			new ComboBoxElement(new CieDe2000Comparison().Compare, "deltaE2000")
		};

		private string allBlocksImage = "";
		private string inputImage = "";
		private string outputImage = "";

		private int progressTotal;

		public MainWindow() {
			InitializeComponent();
			Algorithm_ComboBox.ItemsSource = algs;
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
				inputImage = ofd.FileName;
				InputImage_TextBox.Text = ofd.FileName;
			}
		}

		private void Output_Button_Click(object sender, RoutedEventArgs e) {
			var sfd = new SaveFileDialog {
				Filter = "PNG Image (*.png)|*.png",
				InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
			};
			if (sfd.ShowDialog() == true) {
				outputImage = sfd.FileName;
				OutputImage_TextBox.Text = sfd.FileName;
			}
		}

		private void Start_Button_Click(object sender, RoutedEventArgs e) {
			if (allBlocksImage != "" && inputImage != "" && outputImage != "" && Algorithm_ComboBox.SelectedIndex != -1) {
				Converter.Start(allBlocksImage, inputImage, outputImage, algs[Algorithm_ComboBox.SelectedIndex].Algorithm, ProgressBarUpdate);
			} else {
				MessageBox.Show("Please select all images and the algorithm!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void ProgressBarUpdate(double value, string text) {
			ConversionProgress_ProgressBar.Value = value;
			ConversionProgress_TextBox.Text = text;
		}
	}
}
