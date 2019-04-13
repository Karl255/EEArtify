using ColorMine.ColorSpaces;

namespace EEArtify {
	public class ComboBoxElement {

		public ComparisonAlgorithm Algorithm;

		public string DisplayName { get; set; }

		public ComboBoxElement(ComparisonAlgorithm alg, string displayName) {
			Algorithm = alg;
			DisplayName = displayName;
		}
	}
}
