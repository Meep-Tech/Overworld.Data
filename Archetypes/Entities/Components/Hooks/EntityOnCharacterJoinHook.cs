using Meep.Tech.Data;

namespace Overworld.Data.Entites.Components {
  /// <summary>
  /// A hook that's executed for the joining charachter when they join the world.
  /// </summary>
  public class EntityOnCharacterJoinHook : EntityHook<EntityOnCharacterJoinHook> {
    ///<summary><inheritdoc/></summary>
    public override string SimpleUxMenuTitle 
      => "Hook: On Character Joined";
  }
}
