using System;
using System.Collections.Generic;
using System.Text;

namespace Overworld.Data {
  public interface IPortable {
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
