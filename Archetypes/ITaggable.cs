using System.Collections.Generic;

namespace Overworld.Data {

  /// <summary>
  /// Represents an archetype that's taggable, and has it's own set of default tags.
  /// </summary>
  public interface ITaggable {

    /// <summary>
    /// The default set of tags that should be applied to all models produced by this archetype
    /// </summary>
    public IEnumerable<Tag> DefaultTags {
      get;
    }
  }
}