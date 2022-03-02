using Meep.Tech.Data;

namespace Overworld.Data {

  /// <summary>
  /// A type of model that represents an entity sprite icon, animation, or some kind of display state that can be used to represent and entity though it's sprite.
  /// </summary>
  public interface IEntityDisplayableSprite : IUnique {

    /// <summary>
    /// An archetype that produces an IEntityDisplayableSprite
    /// </summary>
    public interface IArchetype : IFactory {

      /// <summary>
      /// Make the default model
      /// </summary>
      public IEntityDisplayableSprite Make();
    }
  }
}