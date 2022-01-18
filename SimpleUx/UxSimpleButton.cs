using System;

namespace Overworld.Ux.Simple {

  /// <summary>
  /// Represents a simple button in a ui
  /// </summary>
  public class UxSimpleButton : UxDataField {

    /// <summary>
    /// Make a clickable UI button that does something on click.
    /// </summary>
    public UxSimpleButton(
      DisplayType type,
      string name,
      Overworld.Data.Executeable onClick,
      string tooltip = null,
      string dataKey = null,
      bool isReadOnly = false,
      Func<UxPannel, bool> enable = null
    ) : base(
      type,
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