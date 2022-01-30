using System;

namespace Overworld.Ux.Simple {

  /// <summary>
  /// A slider for a value between the min and max.
  /// </summary>
  public class RangeSliderField : DataField {

    /// <summary>
    /// The valid range for this slider
    /// </summary>
    public (float min, float max) ValidRange {
      get;
    }

    /// <summary>
    /// If this only allows whole numbers
    /// </summary>
    public bool IsClampedToWholeNumbers {
      get;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public RangeSliderField(
      string name,
      float min,
      float max,
      bool clampedToWholeNumbers = false,
      string tooltip = null,
      float? value = null,
      string dataKey = null,
      bool isReadOnly = false,
      Func<DataField, View, bool> enabledIf = null
    ) : base(
      DisplayType.RangeSlider,
      name,
      tooltip,
      clampedToWholeNumbers ? Math.Floor(value ?? min) : value ?? min,
      dataKey,
      isReadOnly,
      enabledIf,
      clampedToWholeNumbers ? ((int)Math.Floor(min), (int)Math.Floor(max)) : (min, max)
    ) {
      IsClampedToWholeNumbers = clampedToWholeNumbers;
      ValidRange = clampedToWholeNumbers ? ((int)Math.Floor(min), (int)Math.Floor(max)) : (min, max);
    }
  }
}
