using System;

namespace Overworld.Ux.Simple {
  /// <summary>
  /// A boolean toggle specific Simple Ux Field.
  /// </summary>
  public class UxToggleField : UxDataField {

    /// <summary>
    /// Placeholder text for the input
    /// </summary>
    public string PlaceholderText {
      get;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public UxToggleField(
      string name,
      string tooltip = null, 
      object value = null,
      string dataKey = null,
      bool isReadOnly = false, 
      Func<UxDataField, UxView, bool> enabledIf = null,
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
