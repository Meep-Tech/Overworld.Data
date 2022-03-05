using Meep.Tech.Data;

namespace Overworld.Data {

  /// <summary>
  /// Tags used to find things of many kins
  /// </summary>
  public partial class Tag : Enumeration<Tag> {

    ///<summary>
    /// Used to easily make a new tag. Tags must be unique!
    /// </summary>
    public Tag(string name)
      : base(name.ToUpper()) { }
  }
}
