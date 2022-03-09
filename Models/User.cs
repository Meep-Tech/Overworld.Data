namespace Overworld.Data {
  public class User {

    /// <summary>
    /// The unique, human readable name of a User. The username
    /// </summary>
    public string UniqueName {
      get;
      internal set;
    }

    public User(string uniqueName) {
      UniqueName = uniqueName;
    }
  }
}