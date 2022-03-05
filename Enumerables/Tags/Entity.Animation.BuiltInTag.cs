namespace Overworld.Data {
  public partial class Entity {
    public partial class Animation {
      /// <summary>
      /// Tags used to find entity specific animations
      /// </summary>
      public partial class BuiltInTag : Data.Tag {

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

        /// <summary>
        /// Represents a sprite action used to move a number of tiles from your current position.
        /// For animation that stay in the same place but have motion, use the "Animated" tag.
        /// </summary>
        public readonly static BuiltInTag Move
          = new ("Move");

        /// <summary>
        /// Represents walking
        /// </summary>
        public readonly static BuiltInTag Walk
          = new ("Walk");

        /// <summary>
        /// Represents running
        /// </summary>
        public readonly static BuiltInTag Run
          = new ("Run");

        /// <summary>
        /// Represents jumping
        /// </summary>
        public readonly static BuiltInTag Jump
          = new ("Jump");

        ///<summary><inheritdoc/></summary>
        public BuiltInTag(string name)
          : base(name) { }
      }
    }
  }
}