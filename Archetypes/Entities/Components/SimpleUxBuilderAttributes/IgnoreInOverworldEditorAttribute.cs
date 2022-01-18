namespace Overworld.Data.Entites.Components {
  /// <summary>
  /// Hide a field  with a public get and any setter from the Overworld Editor
  /// </summary>
  [System.AttributeUsage(
    validOn: System.AttributeTargets.Field | System.AttributeTargets.Property,
    Inherited = true
  )] public class IgnoreInOverworldEditorAttribute : System.Attribute {}
}
