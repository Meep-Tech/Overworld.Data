namespace Overworld.Ux.Simple {

  public partial class UxPannel {

    /// <summary>
    /// A tab used to switch between pannels of a simple ux.
    /// </summary>
    public struct TabData {

      /// <summary>
      /// Tab optional key, name is used by default
      /// </summary>
      public readonly string Key;

      /// <summary>
      /// Tab display name
      /// </summary>
      public readonly string Name;

      /// <summary>
      /// If you want an icon, an can be placed in your mod package containing this Ux Pannel, and the url after mods/$PackageName$/ should go here.
      /// </summary>
      public readonly string ImageLocationWithinModPackageFolder;

      /// <summary>
      /// Make a new set of tab data
      /// </summary>
      /// <param name="name"></param>
      /// <param name="key"></param>
      /// <param name="imageLocationWithinModPackageFolder"></param>
      public TabData(string name, string key = null, string imageLocationWithinModPackageFolder = null) {
        Name = name;
        Key = key ?? name;
        ImageLocationWithinModPackageFolder = imageLocationWithinModPackageFolder;
      }
    }
  }
}