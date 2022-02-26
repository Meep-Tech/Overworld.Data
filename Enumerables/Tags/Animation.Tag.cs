using Meep.Tech.Data;

namespace Overworld.Data {
  public partial class Animation {

    /// <summary>
    /// Tags used to find any aimations
    /// </summary>
    public class Tag : Enumeration<Tag> {

      /// <summary>
      /// Indicates North Facing Sprite Animation
      /// </summary>
      public static Tag North { get; }
        = new("North");

      /// <summary>
      /// Indicates South Facing Sprite Animation
      /// </summary>
      public static Tag South { get; }
        = new("South");

      /// <summary>
      /// Indicates Eastward Facing Sprite Animation
      /// </summary>
      public static Tag East { get; }
        = new("East");

      /// <summary>
      /// Indicates Westward Facing Sprite Animation
      /// </summary>
      public static Tag West { get; }
        = new("West");

      /// <summary>
      /// Represents a sprite that is animated
      /// </summary>
      public static Tag Animated { get; }
        = new("Animated");

      /// <summary>
      /// Represents a single framed sprite or A non-moving one
      /// </summary>
      public static Tag Still { get; }
        = new("Still");

      public Tag(string name)
        : base(name.ToUpper()) { }
    }
  }
}
