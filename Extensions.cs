using Simple.Ux.Data;
using System;
using System.Text.RegularExpressions;

namespace Overworld.Utility {

  public static class Extensions {

    /// <summary>
    /// Make a string from "CamelCase" to "Display Case"
    /// </summary>
    public static string ToDisplayCase(this string @string)
      => Regex.Replace(@string, "(?<!^)([A-Z][a-z]|(?<=[a-z])[A-Z])", " $1").Trim('_').Replace("_", " ");
  }
}
