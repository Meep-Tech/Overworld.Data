using System;
using System.Collections.Generic;
using System.Linq;

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

    public RangeSliderField(
      string name,
      float min,
      float max,
      bool clampedToWholeNumbers = false,
      string tooltip = null,
      float? value = null,
      string dataKey = null,
      bool isReadOnly = false,
      Func<DataField, View, bool> enabledIf = null,
      params Func<DataField, double, (bool success, string message)>[] validations
    ) : this(
      name,
      min,
      max,
      clampedToWholeNumbers,
      tooltip,
      value,
      dataKey,
      isReadOnly,
      enabledIf,
      validations?.AsEnumerable()
    ) {
      IsClampedToWholeNumbers = clampedToWholeNumbers;
      ValidRange = clampedToWholeNumbers ? ((int)Math.Floor(min), (int)Math.Floor(max)) : (min, max);
    }

    public RangeSliderField(
      string name,
      float min,
      float max,
      bool clampedToWholeNumbers = false,
      string tooltip = null,
      float? value = null,
      string dataKey = null,
      bool isReadOnly = false,
      Func<DataField, View, bool> enabledIf = null,
      IEnumerable<Func<DataField, double, (bool success, string message)>> validations = null
    ) : base(
      DisplayType.RangeSlider,
      name,
      tooltip,
      clampedToWholeNumbers ? Math.Floor(value ?? min) : value ?? min,
      dataKey,
      isReadOnly,
      enabledIf,
      (validations
        ?? Enumerable.Empty<Func<DataField, double, (bool, string)>>())
          .Append((f, v) => (v >= min && v <= max) ? (true, null) : (false, $"Value: {v}, is outside of valid range: {min} to {max}"))
          .Select(func => func.CastMiddleType<double, object>())
    ) {
      IsClampedToWholeNumbers = clampedToWholeNumbers;
      ValidRange = clampedToWholeNumbers ? ((int)Math.Floor(min), (int)Math.Floor(max)) : (min, max);
    }

    public override bool TryToSetValue(object value, out string message) {
      double number = Math.Round(
          double.TryParse(
            value?.ToString()
              ?? "",
            out double d
          )
            ? d
            : 0,
          2
        );
      value = number;
      return base.TryToSetValue(value, out message);
    }
  }

  internal static class FuncExtensions {
    internal static Func<DataField, TTo, (bool, string)> CastMiddleType<TFrom, TTo>(this Func<DataField, TFrom, (bool, string)> from)
      => (f, v) => from(f, v is TFrom fromType ? fromType : throw new Exception($"Cannot cast from {typeof(TTo).FullName}[TTo] to {typeof(TFrom).FullName}[TFrom]."));
  }
}
