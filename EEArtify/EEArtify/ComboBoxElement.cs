using ColorMine.ColorSpaces;

namespace EEArtify {
	public class ComboBoxElement {

		public ComparisonAlgorithms Algorithm;

		public string DisplayName { get; set; }

		public ComboBoxElement(ComparisonAlgorithms alg, string displayName) {
			Algorithm = alg;
			DisplayName = displayName;
		}
	}
}
