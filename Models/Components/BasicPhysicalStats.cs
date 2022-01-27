using Meep.Tech.Data;
using Overworld.Ux.Simple;
using System;
using System.Collections.Generic;

namespace Overworld.Data.Entities.Components {

  /// <summary>
  /// Some basic physical stats for all characters
  /// </summary>
  public class BasicPhysicalStats : Entity.Component<BasicPhysicalStats> {

    /// <summary>
    /// Height, in "Tiles" (1.75 is average)
    /// </summary>
    [MinimumValue(0.1f)]
    [Tooltip("The character's height in world. Changing this will alter how your character looks and interacts with the world! 1.75 is average.")]
    public float Height {
      get => _height;
      set {
        float toSet = value;
        foreach((_, Func<float, float, float> action) in OnHeightChangeFuncs) {
          toSet += action(_height, value);
        }
        _height = toSet;
      }
    } float _height
      = 1.75f;

    /// <summary>
    /// Width, in "Tiles" (0.75 is average)
    /// </summary>
    [MinimumValue(0.1f)]
    [Tooltip("The character's width in world. Changing this will alter how your character looks and interacts with the world! 0.75 is average.")]
    public float Width {
      get => _width;
      set {
        float toSet = value;
        foreach((_, Func<float, float, float> action) in OnWidthChangeFuncs) {
          toSet += action(_width, value);
        }
        _width = toSet;
      }
    } float _width
      = 0.75f;

    /// <summary>
    /// Weight in "Units", used for physics and some other things.
    /// 175u is average.
    /// </summary>
    [MinimumValue(0.001f)]
    [Tooltip("The character's height in world. Changing this may alter some physics. 175 is average.")]
    public float Weight {
      get => _weight;
      set {
        float toSet = value;
        foreach((_, Func<float, float, float> action) in OnWeightChangeFuncs) {
          toSet += action(_weight, value);
        }
        _weight = toSet;
      }
    } float _weight
      = 175;

    /// <summary>
    /// Add an Func to be executed when the width is changed.
    /// </summary>
    /// Params:
    /// old width,
    /// new width.
    /// Returns:
    /// extra width to add.
    /// </param>
    public readonly DelegateCollection<Func<float, float, float>>
      OnWidthChangeFuncs
        = new();

    /// <summary>
    /// Add an Func to be executed when the weight is changed.
    /// </summary>
    /// Params:
    /// old weight,
    /// new weight.
    /// Returns:
    /// extra weight to add.
    /// </param>    
    public readonly DelegateCollection<Func<float, float, float>> 
      OnWeightChangeFuncs
        = new();

    /// Add an Func to be executed when the height is changed.
    /// </summary>
    /// Params:
    /// old height,
    /// new height.
    /// Returns:
    /// extra height 
    public readonly DelegateCollection<Func<float, float, float>>
      OnHeightChangeFuncs 
        = new();
  }
}
