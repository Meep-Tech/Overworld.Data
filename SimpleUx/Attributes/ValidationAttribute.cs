using System;

namespace Overworld.Ux.Simple {

  /// <summary>
  /// Denotes a method that should be auto rendered as a button.
  /// A valid method takes either no parameters, or a UxSimpleButton(the one clicked on) and a UXPannel as parameters.
  /// TODO: impliment
  /// </summary>
  [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
  public class SimpleButtonAttribute : Attribute {
    internal string _buttonName;

    public SimpleButtonAttribute(string Name = null) {
      _buttonName = null;
      throw new NotImplementedException();
    }
  }

  /// <summary>
  /// Denotes how the field should be validated.
  /// You can name a method that takes one argument and returns a bool,
  /// or for numbers you can provide (int min, int max) as a tuple.
  /// </summary>
  [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
  public class ValidationAttribute : Attribute {
    internal object _validation;

    public ValidationAttribute(object Validation) {
      _validation = Validation;
    }
  }

  /// <summary>
  /// Denotes another field that indicates when this field should be enabled.
  /// The other field must have a boolean type and be a property of field.
  /// </summary>
  [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
  public class EnableIfAttribute : Attribute {
    internal string _validationFieldName;

    public EnableIfAttribute(string validationFieldName) {
      _validationFieldName = validationFieldName;
    }
  }
}