using System;
using System.Collections.Generic;

namespace Overworld.Ux.Simple {
  /// <summary>
  /// Used to indicate a field where you can select one of a set of options.
  /// </summary>
  public class SelectableAttribute : Attribute {
    internal bool _isMultiselect;
    internal IEnumerable<string> _options;

    public SelectableAttribute(bool MultiSelect = false, IEnumerable<string> Options = null) {
      _options = Options;
      _isMultiselect = MultiSelect;
    }
  }
}