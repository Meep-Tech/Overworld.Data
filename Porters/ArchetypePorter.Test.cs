using System.IO;

namespace Overworld.Data.IO {

  public abstract partial class ArchetypePorter<TArchetype> where TArchetype : Meep.Tech.Data.Archetype, IPortableArchetype {
    /// <summary>
    /// A test of a porter and it's abilities
    /// </summary>
    public class Test {

      /// <summary>
      /// The porter to use in testing.
      /// </summary>
      public ArchetypePorter<TArchetype> Porter {
        get;
      }

      /// <summary>
      /// The mods folder to use for testing imports
      /// </summary>
      public string TestModsFolder {
        get => _testModsFolder ?? Path.Combine(RootModsFolder, Porter.DefaultPackageName, "_test");
        init => _testModsFolder = value;
      } string _testModsFolder;

      /// <summary>
      /// Make a new import tester.
      /// </summary>
      public Test(ArchetypePorter<TArchetype> porter) {
        Porter = porter;
      }

      /// <summary>
      /// Don't call this in a runtime with an existing default universe.
      /// </summary>
      public void Run() {

      }
    }
  }
}
