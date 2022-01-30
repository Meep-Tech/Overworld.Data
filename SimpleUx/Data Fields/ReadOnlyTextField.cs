using System;
using System.Collections.Generic;
using System.Text;

namespace Overworld.Ux.Simple {

  /// <summary>
  /// A Text specific Simple Ux Field.
  /// Used just to display text
  /// </summary>
  public class ReadOnlyTextField : TextField {

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public ReadOnlyTextField(
      object text,
      string title = null,
      string tooltip = null,
      string dataKey = null
    ) : base(
      title,
      " ",
      tooltip,
      text,
      dataKey
        ?? (!string.IsNullOrWhiteSpace(title)
          ? title
          : new Guid().ToString()),
      true,
      null,
      null
    ) {
    }
  }
}
