namespace Overworld.Data {
  public partial class Entity {
    public partial class Animation {

      /// <summary>
      /// Tags used to find entity specific animations
      /// </summary>
      public partial class Tag : Data.Animation.Tag {

        /// <summary>
        /// Represents a sprite action used to move a number of tiles from your current position.
        /// For animation that stay in the same place but have motion, use the "Animated" tag.
        /// </summary>
        public readonly static Tag Move
          = new ("Move");

        /// <summary>
        /// Represents walking
        /// </summary>
        public readonly static Tag Walk
          = new ("Walk");

        /// <summary>
        /// Represents running
        /// </summary>
        public readonly static Tag Run
          = new ("Run");

        /// <summary>
        /// Represents jumping
        /// </summary>
        public readonly static Tag Jump
          = new ("Jump");

        public Tag(string name)
          : base(name) { }
      }
    }
  }
}