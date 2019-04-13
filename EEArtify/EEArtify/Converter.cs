using ColorMine.ColorSpaces;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;

namespace EEArtify {

	public delegate void progressUpdater(double value, string text);

	public static class Converter {

		private static Bitmap allBlocksImage;
		private static Bitmap inputImage;
		private static Bitmap outputImage;

		private static List<Color> allowedColors = new List<Color> {
			Color.FromArgb(0, 0, 0)
		};

		public static void Start(string allBlocksImagePath, string inpuImagetPath, string outputImagePath, ComparisonAlgorithm Compare, progressUpdater updateProgressBar) {

			//opens the AllBlocks image
			try {
				allBlocksImage = new Bitmap(allBlocksImagePath);
			} catch {
				MessageBox.Show("Could not open the AllBlocks image!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}

			//opens the input image
			try {
				inputImage = new Bitmap(inpuImagetPath);
			} catch {
				MessageBox.Show("Could not open the input image!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}

			int totalPixels = inputImage.Height * inputImage.Width;

			//takes all colors from the AllBlocks image and puts it in a list
			for (int y = 0; y < inputImage.Height; y++) {
				for (int x = 0; x < inputImage.Width; x++) {
					Color c = inputImage.GetPixel(x, y);
					if (!c.Equals(Color.FromArgb(0, 0, 0))) {
						allowedColors.Add(c);
					}
				}
			}

			//for each pixel, finds the most similar color using the specified Compare method
			for (int y = 0; y < inputImage.Height; y++) {
				for (int x = 0; x < inputImage.Width; x++) {

					double minDeltaE = double.MaxValue;
					var bestColor = Color.FromArgb(0, 0, 0);

					for (int i = 1; i < allowedColors.Count; i++) {
						var c1 = new Rgb() {
							R = inputImage.GetPixel(x, y).R,
							G = inputImage.GetPixel(x, y).G,
							B = inputImage.GetPixel(x, y).B
						};

						var c2 = new Rgb() {
							R = allowedColors[i].R,
							G = allowedColors[i].G,
							B = allowedColors[i].B
						};

						double deltaE = Compare(c1, c2);

						if (deltaE < minDeltaE) {
							minDeltaE = deltaE;
							bestColor = allowedColors[i];
						}
					}

					inputImage.SetPixel(x, y, bestColor);

					int currentPixel = y * inputImage.Height + x;
					if (currentPixel != totalPixels) {
						updateProgressBar(currentPixel / totalPixels, $"Processing... { currentPixel }/{ totalPixels }");
					} else {
						updateProgressBar(1, $"Done! { totalPixels }/{ totalPixels }");
					}
				}
			}

			try {
				inputImage.Save(outputImagePath);
			} catch {
				MessageBox.Show("An error occured while trying to save the file!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}

			MessageBox.Show("Conversion finished!", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
		}
	}
}
