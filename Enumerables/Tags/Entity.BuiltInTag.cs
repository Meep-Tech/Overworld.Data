namespace Overworld.Data {

  public partial class Entity {
    /// <summary>
    /// Built in entity tags.
    /// </summary>
    public class BuiltInTag : Tag {

      ///<summary><inheritdoc/></summary>
      public BuiltInTag(string name)
        : base(name) { }
    }
  }
}
