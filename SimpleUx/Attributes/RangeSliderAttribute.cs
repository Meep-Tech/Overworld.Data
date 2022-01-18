using System;

namespace Overworld.Ux.Simple {

  /// <summary>
  /// Denotes a numeric field that should appear as a range slider in the UI
  /// </summary>
  public class RangeSliderAttribute : Attribute {
    internal int _min;
    internal int _max;

    public RangeSliderAttribute(int min, int max) {
      _min = min;
      _max = max;
    }
  }
}