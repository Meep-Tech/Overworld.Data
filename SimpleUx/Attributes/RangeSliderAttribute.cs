using System;

namespace Overworld.Ux.Simple {

  /// <summary>
  /// Denotes a numeric field that should appear as a range slider in the UI
  /// </summary>
  public class RangeSliderAttribute : Attribute {
    internal float _min;
    internal float _max;

    public RangeSliderAttribute(float min, float max) {
      _min = min;
      _max = max;
    }
  }
}