namespace Overworld.Data {
  public interface IPortableArchetype {
    public string ResourceKey {
      get;
    }

    public string PackageKey {
      get;
    }

    public string DefaultPackageKey {
      get;
    }
  }
}
