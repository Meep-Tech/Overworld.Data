using System;
using System.Linq;
using System.Collections.Generic;
using Overworld.Utility;

namespace Overworld.Ux.Simple {
  /// <summary>
  /// Used to indicate a field where you can select one of a set of options.
  /// </summary>
  public class DropdownAttribute : Attribute {
    internal bool _isMultiselect;
    internal Dictionary<string, object> _options;

    /// <summary>
    /// Make a new selectable
    /// </summary>
    /// <param name="MultiSelect">If more than one value can be selected</param>
    /// <param name="OptionValues">The option values. If no names are provided, these are turned into strings and those are used as the field keys</param>
    /// <param name="OptionNames">The option names. Must either have none, or the same number as the values</param>
    public DropdownAttribute(bool MultiSelect = false, object[] OptionValues = null, string[] OptionNames = null) {
      _options = OptionValues is not null 
        ? new Dictionary<string, object>((OptionNames ?? OptionValues.Select(
          value => value.GetType().IsEnum ? value.ToString().ToDisplayCase() : value.ToString()))
          .Zip(OptionValues, (n, v) => new KeyValuePair<string, object>(n, v)))
        : null;
      _isMultiselect = MultiSelect;
    }
  }
}