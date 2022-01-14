using System.Collections.Generic;
using static Meep.Tech.Data.Configuration.Loader.Settings;

namespace Overworld.Data {

  public partial class Character {

    [Branch]
    [DoNotBuildInInitialLoad]
    public new class Type : Entity.Type {
      protected override Dictionary<string, object> DefaultTestParams
        => new() {
          {nameof(Character.UniqueName), "Test" }
        };

      /// <summary>
      /// For X Bam
      /// </summary>
      /// <param name="id"></param>
     protected Type(string resourceId, Identity id) 
        : base(resourceId, id ?? new Identity("Character")) {}
    }
  }
}