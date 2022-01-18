using Meep.Tech.Data;

namespace Overworld.Data {
  public partial class Animation {

    /// <summary>
    /// Tags used to find any aimations
    /// </summary>
    public class Tag : Enumeration<Tag> {

      /// <summary>
      /// Used for the entity's icon.
      /// </summary>
      public readonly static Tag Icon
          = new ("Icon");

      /// <summary>
      /// Indicates North Facing Sprite Animation
      /// </summary>
      public readonly static Tag North
          = new ("North");

      /// <summary>
      /// Indicates South Facing Sprite Animation
      /// </summary>
      public readonly static Tag South
          = new ("South");

      /// <summary>
      /// Indicates Eastward Facing Sprite Animation
      /// </summary>
      public readonly static Tag East
          = new ("East");

      /// <summary>
      /// Indicates Westward Facing Sprite Animation
      /// </summary>
      public readonly static Tag West
          = new ("West");

      /// <summary>
      /// Represents a sprite that is animated
      /// </summary>
      public readonly static Tag Animated
          = new ("Animated");

      /// <summary>
      /// Represents a single framed sprite or A non-moving one
      /// </summary>
      public readonly static Tag Still
          = new ("Still");

      public Tag(string name)
        : base(name.ToUpper()) { }
    }
  }
}
