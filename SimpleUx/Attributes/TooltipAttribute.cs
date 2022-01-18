using System;

namespace Overworld.Ux.Simple {
  /// <summary>
  /// Adds a tooltip to the field.
  /// </summary>
  public class TooltipAttribute : Attribute {
    internal string _text;

    public TooltipAttribute(string text) {
      _text = text;
    }
  }
}