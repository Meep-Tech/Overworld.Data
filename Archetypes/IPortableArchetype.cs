using System;
using System.Collections.Generic;
using System.Text;

namespace Overworld.Data {
  public interface IPortableArchetype {
    public string ResourceKey {
      get;
    }

    public string PackageName {
      get;
    }

    public string DefaultPackageName {
      get;
    }
  }
}
