using Overworld.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Overworld.Ux.Simple {

  /// <summary>
  /// A drop down that can select values
  /// </summary>
  public class DropdownSelectField : DataField {

    /// <summary>
    /// The valid options, indexed by their display key
    /// </summary>
    public IReadOnlyDictionary<string, object> Options
      => _options;
    Dictionary<string, object> _options;

    /// <summary>
    /// If the user can select more than one value
    /// </summary>
    public bool MultiselectAllowed {
      get;
    }

    /// <summary>
    /// The value(s) selected.
    /// </summary>
    public new List<KeyValuePair<string, object>> Value {
      get;
    }

    public DropdownSelectField(
      string name,
      IEnumerable<object> optionValues,
      IEnumerable<string> optionNames = null,
      bool multiselectIsAllowed = false,
      string tooltip = null,
      IEnumerable<string> alreadySelectedOptionKeys = null,
      string dataKey = null,
      bool isReadOnly = false,
      Func<DataField, View, bool> enabledIf = null,
      Func<DataField, KeyValuePair<string, object>, bool> validation = null
    ) : this(
        name,
        new Dictionary<string ,object>((optionNames ?? optionValues.Select(
          value => value.GetType().IsEnum ? value.ToString().ToDisplayCase() : value.ToString())
        ).Zip(optionValues, (n, v) => new KeyValuePair<string, object>(n, v))),
        multiselectIsAllowed,
        tooltip,
        alreadySelectedOptionKeys,
        dataKey,
        isReadOnly,
        enabledIf,
        validation
    ) {
    }

    public DropdownSelectField(
      string name,
      Dictionary<string, object> options,
      bool multiselectIsAllowed = false,
      string tooltip = null,
      IEnumerable<string> alreadySelectedOptionKeys = null,
      string dataKey = null,
      bool isReadOnly = false,
      Func<DataField, View, bool> enabledIf = null,
      Func<DataField, KeyValuePair<string, object>, bool> validation = null
    ) : base(
      DisplayType.Dropdown,
      name,
      tooltip,
      alreadySelectedOptionKeys.Select(key => new KeyValuePair<string,object>(key, options[key])).ToList(),
      dataKey,
      isReadOnly,
      enabledIf,
      (f, v) => validation(f, (KeyValuePair<string, object>)v)
    ) {
      _options = options;
      MultiselectAllowed = multiselectIsAllowed;
    }
  }
}
