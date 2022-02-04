using System;
using System.Collections.Generic;
using System.Linq;

namespace Overworld.Ux.Simple {
  /// <summary>
  /// A boolean toggle specific Simple Ux Field.
  /// </summary>
  public class ToggleField : DataField {

    public ToggleField(
      string name,
      string tooltip = null, 
      bool value = false,
      string dataKey = null,
      bool isReadOnly = false, 
      Func<DataField, View, bool> enabledIf = null,
      params Func<DataField, bool, (bool success, string message)>[] validations
    ) : this(
      name, 
      tooltip,
      value,
      dataKey,
      isReadOnly,
      enabledIf, 
      validations?.AsEnumerable()
    ) {}

    public ToggleField(
      string name,
      string tooltip = null, 
      bool value = false,
      string dataKey = null,
      bool isReadOnly = false, 
      Func<DataField, View, bool> enabledIf = null,
      IEnumerable<Func<DataField, bool, (bool success, string message)>> validations = null
    ) : base(
      DisplayType.Toggle, 
      name, 
      tooltip,
      value,
      dataKey,
      isReadOnly,
      enabledIf, 
      validations?
        .Select(func => func.CastMiddleType<bool, object>())
    ) {}
  }
}
