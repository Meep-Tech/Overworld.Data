using System;
using System.Linq;
using System.Collections.Generic;

namespace Overworld.Ux.Simple {

  /// <summary>
  /// A Text specific Simple Ux Field.
  /// Can be used for input, or Read Only for just display text.
  /// </summary>
  public class TextField : DataField {

    /// <summary>
    /// Placeholder text for the input
    /// </summary>
    public string PlaceholderText {
      get;
    }

    public TextField(
      string name,
      string placeholderText = null,
      string tooltip = null,
      object value = null,
      string dataKey = null,
      bool isReadOnly = false,
      Func<DataField, View, bool> enabledIf = null,
      params Func<DataField, string, (bool success, string message)>[] validations
    ) : this(
      name,
      placeholderText,
      tooltip,
      value ?? "",
      dataKey,
      isReadOnly,
      enabledIf,
      validations?.AsEnumerable()
    ) {
      PlaceholderText = placeholderText;
    }

    public TextField(
      string name,
      string placeholderText = null,
      string tooltip = null,
      object value = null,
      string dataKey = null,
      bool isReadOnly = false,
      Func<DataField, View, bool> enabledIf = null,
      IEnumerable<Func<DataField, string, (bool success, string message)>> validations = null
    ) : base(
      DisplayType.Text,
      name,
      tooltip,
      value ?? "",
      dataKey,
      isReadOnly,
      enabledIf,
      validations?
        .Select(func => func.CastMiddleType<string, object>())
    ) {
      PlaceholderText = placeholderText;
    }
  }
}
