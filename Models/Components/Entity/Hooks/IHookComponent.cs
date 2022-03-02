using System.Collections.Generic;

namespace Overworld.Data.Entites.Components {
  internal interface IHookComponent {
    IEnumerable<Executeable> Executeables {
      get;
    }
  }
}