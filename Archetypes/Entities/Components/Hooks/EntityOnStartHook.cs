using Meep.Tech.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Overworld.Data.Entites.Components {

  /// <summary>
  /// A hook executred on the world being started by the server.
  /// </summary>
  public class EntityOnStartHook : EntityHook<EntityOnStartHook> {
    static EntityOnStartHook() {
      Archetypes<Type>._.DisplayName = "Hook: On World StartUp";
    }
  }
}
