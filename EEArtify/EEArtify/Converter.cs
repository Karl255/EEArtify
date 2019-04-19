using ColorMine.ColorSpaces;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using System.Windows.Threading;
using System.Threading;
using ColorMine.ColorSpaces.Comparisons;
using System.Threading.Tasks;

namespace EEArtify {

	//public delegate void ProgressBarUpdater();

	public class Converter {

		/*
		public int TotalWork { get; private set; } = -1;
		
		private int _progress = -1;
		public int Progress {
			get { return _progress; }
			private set {
				_progress = value;
				Dispatcher.Invoke(() => UpdateProgressBar());
			}
		}

		public ProgressBarUpdater UpdateProgressBar = () => {};
		public Dispatcher Dispatcher;
		*/

		private List<Color> AllowedColors;
		
		public Bitmap Image {
			get;
			private set;
		} = new Bitmap(1, 1);

		private ComparisonAlgorithms Comparator;

		private Dictionary<ComparisonAlgorithms, ComparisonAlgorithm> algs = new Dictionary<ComparisonAlgorithms, ComparisonAlgorithm> {
			{ ComparisonAlgorithms.DeltaE76, new Cie1976Comparison().Compare },
			{ ComparisonAlgorithms.DeltaE94, new Cie94Comparison().Compare },
			{ ComparisonAlgorithms.CMCcl, new CmcComparison().Compare },
			{ ComparisonAlgorithms.DeltaE2000, new CieDe2000Comparison().Compare }
		};

		public async Task Start(List<Color> allowedColors, Bitmap image, ComparisonAlgorithms comparator) {
			AllowedColors = allowedColors;
			Image = image;
			Comparator = comparator;
			await Task.Run(() => StartAsync());
		}

		private void StartAsync() {
			ComparisonAlgorithm Compare = algs[Comparator];

			//TotalWork = image.Height * image.Width;
			
			//for each pixel at (x, y), finds the most similar color in the allowedColor List using the specified Compare method
			for (int y = 0; y < Image.Height; y++) {
				for (int x = 0; x < Image.Width; x++) {

					double minDeltaE = double.MaxValue;
					var bestColor = Color.FromArgb(0, 0, 0); //defaults to black

					for (int i = 0; i < AllowedColors.Count; i++) {
						var c1 = new Rgb() {
							R = Image.GetPixel(x, y).R,
							G = Image.GetPixel(x, y).G,
							B = Image.GetPixel(x, y).B
						};

						var c2 = new Rgb() {
							R = AllowedColors[i].R,
							G = AllowedColors[i].G,
							B = AllowedColors[i].B
						};

						double deltaE = Compare(c1, c2);

						if (deltaE < minDeltaE) {
							minDeltaE = deltaE;
							bestColor = AllowedColors[i];
						}
					}

					Image.SetPixel(x, y, bestColor);
					//Progress++;
				}
			}
			
		}

		/*
		public void AddUpdater(Dispatcher dis, ProgressBarUpdater del) {
			Dispatcher = dis;
			UpdateProgressBar = del;
		}
		*/
	}

	public enum ComparisonAlgorithms {
		DeltaE76,
		DeltaE94,
		CMCcl,
		DeltaE2000
	}
}
