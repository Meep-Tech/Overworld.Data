using System;
using System.Collections.Generic;
using System.Text;

namespace Overworld.Ux.Simple {

  /// <summary>
  /// A Text specific Simple Ux Field.
  /// Can be used for input, or Read Only for just display text.
  /// </summary>
  public class UxTextField : UxDataField {

    /// <summary>
    /// Placeholder text for the input
    /// </summary>
    public string PlaceholderText {
      get;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public UxTextField(
      string name,
      string placeholderText = null,
      string tooltip = null,
      object value = null,
      string dataKey = null,
      bool isReadOnly = false,
      Func<UxDataField, UxView, bool> enabledIf = null,
      Func<string, bool> validation = null
    ) : base(
      DisplayType.Text,
      name,
      tooltip,
      value,
      dataKey,
      isReadOnly,
      enabledIf,
      validation
    ) {
      PlaceholderText = placeholderText;
    }
  }
}
