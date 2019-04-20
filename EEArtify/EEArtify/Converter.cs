using ColorMine.ColorSpaces;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using System.Windows.Threading;
using System.Threading;
using ColorMine.ColorSpaces.Comparisons;
using System.Threading.Tasks;

namespace EEArtify {

	public delegate void ProgressBarUpdaterDelegate(int progress);

	public class Converter {

		private LinkedList<Color> _allowedColors;
		public Bitmap Image { get; private set; } = new Bitmap(1, 1);
		public int TotalWork { get; private set; } = -1;
		
		//each time progress is made, update the progress bar
		private int _progress = -1;
		public int Progress {
			get => _progress;
			private set {
				if (hasUpdater) {
					_progress = value;
					Dispatcher.Invoke(() => UpdateProgressBar(_progress));
				}
			}
		}

		private ComparisonAlgorithm Compare;

		private ProgressBarUpdaterDelegate UpdateProgressBar;
		public Dispatcher Dispatcher;
		private bool hasUpdater = false;

		private Dictionary<ComparisonAlgorithms, ComparisonAlgorithm> algs = new Dictionary<ComparisonAlgorithms, ComparisonAlgorithm> {
			{ ComparisonAlgorithms.DeltaE76, new Cie1976Comparison().Compare },
			{ ComparisonAlgorithms.DeltaE94, new Cie94Comparison().Compare },
			{ ComparisonAlgorithms.CMCcl, new CmcComparison().Compare },
			{ ComparisonAlgorithms.DeltaE2000, new CieDe2000Comparison().Compare }
		};

		public async Task StartAsync(LinkedList<Color> allowedColors, Bitmap image, ComparisonAlgorithms comparator) {
			_allowedColors = allowedColors;
			Image = image;
			Compare = algs[comparator];
			
			await Task.Run(() => Start());
		}

		private void Start() {
			
			var imageColors = new LinkedList<Color>();
			
			TotalWork = Image.Height * Image.Width;
			Progress = 0;

			//puts all colors of an image into a LinkedList
			for (int y = 0; y < Image.Height; y++) {
				for(int x = 0; x < Image.Width; x++) {
					var c = Image.GetPixel(x, y);
					if (!imageColors.Contains(c))
						imageColors.AddLast(c);
					Progress++;
				}
			}

			var mappedColors = new Dictionary<Color, Color>();

			TotalWork = _allowedColors.Count;
			Progress = 0;

			//finds the closest color for each color in the imageColors LinkedList
			foreach (var imageColor in imageColors) {
				double minDeltaE = double.MaxValue;
				var bestColor = Color.FromArgb(0, 0, 0);
				var imageColorRgb = new Rgb() {
					R = imageColor.R,
					G = imageColor.G,
					B = imageColor.B
				};
				
				foreach (var allowedColor in _allowedColors) {
					var allowedColorRgb = new Rgb() {
						R = allowedColor.R,
						G = allowedColor.G,
						B = allowedColor.B
					};
					
					double deltaE = Compare(imageColorRgb, allowedColorRgb);

					if (deltaE < minDeltaE) {
						minDeltaE = deltaE;
						bestColor = allowedColor;
					}
				}
				mappedColors.Add(imageColor, bestColor);
				Progress++;
			}

			TotalWork = Image.Width * Image.Height;
			Progress = 0;

			//applies the allowed colors to the image
			for (int y = 0; y < Image.Height; y++) {
				for (int x = 0; x < Image.Width; x++) {
					Image.SetPixel(x, y, mappedColors[Image.GetPixel(x, y)]);
					Progress++;
				}
			}
		}
		
		public void AddUpdater(Dispatcher dis, ProgressBarUpdaterDelegate del) {
			Dispatcher = dis;
			UpdateProgressBar = del;
			hasUpdater = true;
		}
	}

	public enum ComparisonAlgorithms {
		DeltaE76,
		DeltaE94,
		CMCcl,
		DeltaE2000
	}
}
