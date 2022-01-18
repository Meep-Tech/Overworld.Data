namespace Overworld.Data {
  public partial class Entity {
    public partial class Animation {
      /// <summary>
      /// The layer of the entity an animation applies to
      /// </summary>
      public enum Layer {
        Underlay = -1,
        BaseBody,
        Overlay,
        OverallEffect = '*'
      }
    }
  }
}