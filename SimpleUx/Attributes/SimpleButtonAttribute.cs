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
}