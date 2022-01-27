﻿using System;

namespace Overworld.Ux.Simple {
  /// <summary>
  /// Denotes another field that indicates when this field should be enabled.
  /// The other field must have a boolean type and be a property of field, or the field could be a Func[UxField, UxView]
  /// </summary>
  [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
  public class EnableIfAttribute : Attribute {
    internal string _validationFieldName;

    public EnableIfAttribute(string validationFieldName) {
      _validationFieldName = validationFieldName;
    }
  }
}