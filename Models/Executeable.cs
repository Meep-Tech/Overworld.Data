
using Meep.Tech.Data;

namespace Overworld.Data {

  /// <summary>
  /// An item that can be added to a component signifying logic that can be executed.
  /// Currently just OWS scripts.
  /// </summary>
  public partial class Executeable : Model<Executeable, Executeable.Type>, IModel.IUseDefaultUniverse {

    /// <summary>
    /// A type of executable, setable in the editor.
    /// </summary>
    public abstract partial class Type : Archetype<Executeable, Executeable.Type> {

      /// <summary>
      /// For making new types of executables.
      /// </summary>
      /// <param name="id"></param>
      protected Type(Archetype.Identity id) 
        : base(id) { }
    }

    /// <summary>
    /// Execute this with the internal program and context.
    /// </summary>
    public virtual void Execute() {}
  }
}