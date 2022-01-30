using System;

namespace Overworld.Ux.Simple {

  /// <summary>
  /// Represents a simple button in a ui
  /// </summary>
  public class SimpleButton : DataField {

    /// <summary>
    /// Make a clickable UI button that does something on click.
    /// </summary>
    public SimpleButton(
      string name,
      Overworld.Data.Executeable onClick,
      string tooltip = null,
      string dataKey = null,
      bool isReadOnly = false,
      Func<DataField, View, bool> enable = null
    ) : base(
      DisplayType.Button,
      name,
      tooltip,
      onClick, 
      dataKey,
      isReadOnly,
      enable,
      null
    ) {}

    /// <summary>
    /// Used to update the colletction
    /// </summary>
    public void Click() {
      ///onClick.Execute();
    }
  }
}