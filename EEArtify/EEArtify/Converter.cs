using ColorMine.ColorSpaces;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;

namespace EEArtify {

	public delegate void ProgressBarUpdater();

	public class Converter {

		public int TotalWork { get; private set; } = -1;
		
		private int _progress = -1;
		public int Progress {
			get { return _progress; }
			private set {
				_progress = value;
				UpdateProgressBar();
			}
		}
		
		public ProgressBarUpdater UpdateProgressBar = () => {};

		public Bitmap Start(List<Color> allowedColors, Bitmap image, ComparisonAlgorithm Compare, ProgressBarUpdater updater) {
			
			UpdateProgressBar = updater;
			TotalWork = image.Height * image.Width;
			
			//for each pixel at (x, y), finds the most similar color in the allowedColor List using the specified Compare method
			for (int y = 0; y < image.Height; y++) {
				for (int x = 0; x < image.Width; x++) {

					double minDeltaE = double.MaxValue;
					var bestColor = Color.FromArgb(0, 0, 0); //defaults to black

					for (int i = 0; i < allowedColors.Count; i++) {
						var c1 = new Rgb() {
							R = image.GetPixel(x, y).R,
							G = image.GetPixel(x, y).G,
							B = image.GetPixel(x, y).B
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

					image.SetPixel(x, y, bestColor);
					Progress++;
				}
			}
			
			return image;
		}
	}
}
