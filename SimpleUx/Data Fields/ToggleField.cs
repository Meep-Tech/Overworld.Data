using System;

namespace Overworld.Ux.Simple {
  /// <summary>
  /// A boolean toggle specific Simple Ux Field.
  /// </summary>
  public class ToggleField : DataField {

    /// <summary>
    /// Placeholder text for the input
    /// </summary>
    public string PlaceholderText {
      get;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public ToggleField(
      string name,
      string tooltip = null, 
      bool value = false,
      string dataKey = null,
      bool isReadOnly = false, 
      Func<DataField, View, bool> enabledIf = null,
      Func<bool, bool> validation = null
    ) : base(
      DisplayType.Toggle, 
      name, 
      tooltip,
      value,
      dataKey,
      isReadOnly,
      enabledIf, 
      validation
    ) {}
  }
}
