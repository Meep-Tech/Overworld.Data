using Meep.Tech.Data;

namespace Overworld.Data.Entites.Components {

  /// <summary>
  /// A hook executred on the world being started by the server.
  /// </summary>
  public class EntityOnStartHook : EntityHook<EntityOnStartHook> {
    ///<summary><inheritdoc/></summary>
    public override string SimpleUxMenuTitle 
      => "Hook: On World StartUp";
  }
}
