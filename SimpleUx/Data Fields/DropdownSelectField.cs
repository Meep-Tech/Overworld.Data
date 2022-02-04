using Meep.Tech.Collections.Generic;
using Overworld.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Overworld.Ux.Simple {

  /// <summary>
  /// A drop down that can select enum values
  /// </summary>
  public class DropdownSelectField<TEnum> : DropdownSelectField
    where TEnum : Enum {

    public DropdownSelectField(
      string name, 
      bool multiselectIsAllowed = false, 
      string tooltip = null,
      IEnumerable<string> alreadySelectedOptionKeys = null, 
      string dataKey = null, 
      bool isReadOnly = false, 
      Func<DataField, View, bool> enabledIf = null,
      params Func<DataField, KeyValuePair<string, object>, (bool success, string message)>[] validations
    ) : this(
      name,
      multiselectIsAllowed,
      tooltip,
      alreadySelectedOptionKeys,
      dataKey,
      isReadOnly,
      enabledIf,
      validations?.Cast<Func<DataField, KeyValuePair<string, object>, (bool success, string message)>>()
    ) {}

    public DropdownSelectField(
      string name, 
      bool multiselectIsAllowed = false, 
      string tooltip = null,
      IEnumerable<string> alreadySelectedOptionKeys = null, 
      string dataKey = null, 
      bool isReadOnly = false, 
      Func<DataField, View, bool> enabledIf = null,
      IEnumerable<Func<DataField, KeyValuePair<string, object>, (bool success, string message)>> validations = null
    ) : base(
      name,
      Enum.GetValues(typeof(TEnum)).Cast<object>().ToDictionary(e => e.ToString().ToDisplayCase()),
      multiselectIsAllowed,
      tooltip,
      alreadySelectedOptionKeys,
      dataKey,
      isReadOnly,
      enabledIf,
      validations
    ) {}
  }

  /// <summary>
  /// A drop down that can select values
  /// </summary>
  public class DropdownSelectField : DataField {

    /// <summary>
    /// The valid options, indexed by their display key.
    /// READ-ONLY!
    /// </summary>
    public IReadOnlyOrderedDictionary<string, object> Options
      => _options;
    OrderedDictionary<string, object> _options;

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
      params Func<DataField, KeyValuePair<string, object>, (bool success, string message)>[] validations
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
        validations?.AsEnumerable()
    ) {}

    public DropdownSelectField(
      string name,
      Dictionary<string, object> options,
      bool multiselectIsAllowed = false,
      string tooltip = null,
      IEnumerable<string> alreadySelectedOptionKeys = null,
      string dataKey = null,
      bool isReadOnly = false,
      Func<DataField, View, bool> enabledIf = null,
      IEnumerable<Func<DataField, KeyValuePair<string, object>, (bool success, string message)>> validations = null
    ) : base(
      DisplayType.Dropdown,
      name,
      tooltip,
      alreadySelectedOptionKeys?.Select(key => new KeyValuePair<string,object>(key, options[key])).ToList(),
      dataKey,
      isReadOnly,
      enabledIf,
      (validations
        ?? Enumerable.Empty<Func<DataField, KeyValuePair<string, object>, (bool success, string message)>>())
          .Append((f, v) => (f as DropdownSelectField)._options.TryGetValue(v.Key, out object expected) && v.Value == expected 
            ? (true, "")
            : (false, $"Unrecognized Select Item: {v.Key}, with value: {v.Value ?? "null"}. \n Valid Items:\n{string.Join('\n', (f as DropdownSelectField)._options.Keys.Select(key => $"\t> {key}"))}")
          ).Select(func => func.CastMiddleType<KeyValuePair<string, object>, object>())
    ) {
      _options = new OrderedDictionary<string, object>(options);
      MultiselectAllowed = multiselectIsAllowed;
    }

    ///<summary><inheritdoc/></summary>
    public override DataField Copy(View toNewView = null, bool withCurrentValuesAsNewDefaults = false) {
      var value = base.Copy(toNewView, withCurrentValuesAsNewDefaults);
      value.Value = Value.ToList();
      value.DefaultValue = withCurrentValuesAsNewDefaults ? Value.ToList() : (DefaultValue as List<KeyValuePair<string, object>>).ToList();
      (value as DropdownSelectField)._options = new(_options);

      return value;
    }
  }
}
