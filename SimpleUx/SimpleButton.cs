using System;

namespace Overworld.Ux.Simple {

  /// <summary>
  /// Represents a simple clickable button in a ui.
  /// You can add more onClick callbacks via the OnValueChangeListeners callbacks.
  /// </summary>
  public class SimpleButton : DataField<Overworld.Data.Executeable> {

    /// <summary>
    /// Make a clickable UI button that does something on click.
    /// You can add more onClick callbacks via the OnValueChangedListeners callbacks.
    /// </summary>
    public SimpleButton(
      string name,
      string tooltip = null,
      string dataKey = null,
      bool isReadOnly = false,
      Overworld.Data.Executeable onClick = null
    ) : base(
      DisplayType.Button,
      name,
      tooltip,
      onClick, 
      dataKey,
      isReadOnly
    ) {}

    /// <summary>
    /// Used to update the colletction
    /// You can add more onClick callbacks via the OnValueChangedListeners callbacks.
    /// </summary>
    public void Click() {
      Value?.Execute();
      TryToSetValue(Value, out _);
    }
  }
}