using System;
using System.Collections.Generic;

namespace Overworld.Ux.Simple {

  /// <summary>
  /// A data field for input or display in a simple ux pannel/view
  /// </summary>
  public class DataField : IUxViewElement {

    /// <summary>
    /// The view this field is in.
    /// </summary>
    public View View {
      get;
      internal set;
    }

    /// <summary>
    /// The type of displays available for simple ux data types.
    /// </summary>
    public enum DisplayType {
      Text,
      Toggle,
      RangeSlider,
      SelectOneOfManyDropdown,
      SelectManyDropdown,
      FieldList,
      KeyValueFieldList,
      Executeable,
      ColorPicker,
      Image,
      Button
    }

    /// <summary>
    /// The type of display this field should use.
    /// </summary>
    public DisplayType Type {
      get;
    }

    /// <summary>
    /// Used differently for different kinds of fields.
    /// Can also be replaced with a Func that takes the current value as an object and returns a bool, or (bool success, string message) tuple to override default behaviours
    /// </summary>
    public object Validation {
      get;
    }

    /// <summary>
    /// The current value of the field.
    /// Also will be the default initial value.
    /// </summary>
    public object Value {
      get;
      private set;
    }

    /// <summary>
    /// If this field is readonly
    /// </summary>
    public virtual bool IsReadOnly {
      get;
      internal set;
    } = false;

    /// <summary>
    /// The name of the field.
    /// Used as a default data key
    /// </summary>
    public virtual string Name {
      get;
    }

    /// <summary>
    /// Data key for the field.
    /// Used to access it from the editor component display data.
    /// </summary>
    public virtual string DataKey {
      get;
    }

    /// <summary>
    /// Info tooltip for the field
    /// </summary>
    public virtual string Tooltip {
      get;
    } = null;

    /// <summary>
    /// Used to determine if the field should be enabled.
    /// </summary>
    public Func<DataField, View, bool> Enable {
      get;
    }

    internal DataField _controllerField;

    /// <summary>
    /// Make a new data field for a Simple Ux.
    /// </summary>
    /// <param name="type">the DisplayType to use for this field</param>
    /// <param name="name">the field name. should be unique unless you change the data key</param>
    /// <param name="tooltip">a breif description of the field, will appear on mouse hover in the ui</param>
    /// <param name="value">default/current value of the field</param>
    /// <param name="dataKey">Used to get the value of this field from the view</param>
    /// <param name="isReadOnly">Some read only fields may be formatted differently (like Text). try passing '() => false' to enable if you want a blured out input field instead.</param>
    /// <param name="enable">A function to determine if this field should be enabled currently or not. Parameters are this field, and the parent pannel.</param>
    /// <param name="validation">USed differently for different fields to validate data. Can be overriden with a null, or a Func(object Value)</param>
    public DataField(
      DisplayType type,
      string name,
      string tooltip = null,
      object value = null,
      string dataKey = null, 
      bool isReadOnly = false,
      Func<DataField, View, bool> enable = null,
      object validation = null
    ) {
      Type = type;
      Name = name;
      Tooltip = tooltip;
      Value = value;
      IsReadOnly = isReadOnly;
      DataKey = string.IsNullOrWhiteSpace(dataKey)
        ? name
        : dataKey;

      if(!isReadOnly && DataKey is null) {
        throw new ArgumentException($"Non-read-only fields require a data key. Provide a title, name, or datakey to the field constructor or Make function");
      }

      Enable = enable ?? ((_, _) => true);
      Validation = validation;
    }

    /// <summary>
    /// Try to update the field value to a new one.
    /// Checks validations and returns an error message if there is one.
    /// </summary>
    public bool TryToSetValue(object value, out string message) {
      message = "Set Successfully";
      switch(Type) {
        case DisplayType.RangeSlider:
          if(Validation?.GetType() == typeof((float, float))) {
            double number = (double)value;
            (double min, double max) bounds = (((float, float))Validation);
            if(number > bounds.max && number < bounds.min) {
              message = "Number Out Of Range Bounds";
              return false;
            }
          }
          break;
        case DisplayType.Text:
          if(Validation?.GetType() == typeof((float, float))) {
            double number = (double)value;
            (double min, double max) = (((float, float))Validation);
            if(number > max && number < min) {
              message = "Number Out Of Range Bounds";
              return false;
            }
          } else if(Validation?.GetType() == typeof((int, int))) {
            string text = (string)value;
            (double min, double max) = (((float, float))Validation);
            if(text.Length < min || text.Length > max) {
              message = "String length Out Of Range Bounds";
              return false;
            }
          }
          break;
      }

      /// for controller fields, that need to be validated by their parent.
      if(_controllerField is not null) {
        var pair = (KeyValuePair<string, object>)value;
        if(!(_controllerField.Validation as Func<KeyValuePair<string, object>, bool>)
          (pair)) {
          return false;
        }

        (_controllerField as DataFieldKeyValueSet)._update(pair);
      }

      //Default func
      if(Validation is Func<object, bool> validate) {
        if(!validate(value)) {
          message = "Value did not pass custom validation functions.";
          return false;
        }
      }
      if(Validation is Func<object, (bool success, string message)> validateWithMessage) {
        var validation = validateWithMessage(value);
        if(!validation.success) {
          message = validation.message;
          return false;
        }
      }

      Value = value;
      return true;
    }

    /// <summary>
    /// Memberwise clone to copy
    /// </summary>
    /// <returns></returns>
    public DataField Copy(View toNewView = null) {
      var newField = MemberwiseClone() as DataField;
      newField.View = toNewView;

      return newField;
    }

    ///<summary><inheritdoc/></summary>
    IUxViewElement IUxViewElement.Copy(View toNewView)
      => Copy(toNewView);

    /// <summary>
    /// Make a new field that fits your needs
    /// </summary>
    public static DataField Make(
      DisplayType type, 
      string title = null,
      string tooltip = null, 
      object value = null,
      bool isReadOnly = false,
      object validation = null,
      Func<DataField, View, bool> enabledIf = null,
      string dataKey = null
    ) {
      switch(type) {
        case DisplayType.Text:
          if(isReadOnly) {
            return new ReadOnlyTextField(
              title: title,
              tooltip: tooltip,
              text: value,
              dataKey: dataKey
            );
          } else
            return new TextField(
              name: title,
              validation: validation as System.Func<string, bool>,
              tooltip: tooltip,
              value: value,
              enabledIf: enabledIf,
              dataKey: dataKey
            );
        case DisplayType.Toggle:
          bool boolValue = value is bool asBool
            ? asBool
            : float.TryParse(value.ToString(), out float parsedAsFloat) && parsedAsFloat > 0;
          return new ToggleField(
            name: title,
            validation: validation as System.Func<bool, bool>,
            tooltip: tooltip,
            value: boolValue,
            enabledIf: enabledIf,
            dataKey: dataKey
          );
        case DisplayType.RangeSlider:
          bool clamped = false;
          (float min, float max)? minAndMax = null;
          if(validation is (int clampedMin, int clampedMax)) {
            clamped = true;
            minAndMax = (clampedMin, clampedMax);
          } else if(validation is (float min, float max)) {
            minAndMax = (min, max);
          }

          float? floatValue = value is float asFloat
            ? asFloat
            : float.TryParse(value.ToString(), out float parsedFloat)
             ? parsedFloat
             : null;

          return new RangeSliderField(
            name: title,
            min: minAndMax?.min ?? 0,
            max: minAndMax?.max ?? 1,
            clampedToWholeNumbers: clamped,
            tooltip: tooltip,
            value: floatValue,
            enabledIf: enabledIf,
            dataKey: dataKey
          );
        case DisplayType.KeyValueFieldList:
          return new DataFieldKeyValueSet(
            name: title,
            rows: value as Dictionary<string, object>,
            extraEntryValidation: validation as Func<KeyValuePair<string, object>, bool>,
            tooltip: tooltip,
            dataKey: dataKey,
            isReadOnly: isReadOnly,
            enable: enabledIf
          );
        case DisplayType.SelectOneOfManyDropdown:
        case DisplayType.SelectManyDropdown:
        case DisplayType.FieldList:
        case DisplayType.Executeable:
        case DisplayType.ColorPicker:
        case DisplayType.Image:
        case DisplayType.Button:
          throw new NotImplementedException(type.ToString());
        default:
          return new DataField(
            name: title,
            type: type,
            validation: validation,
            tooltip: tooltip,
            value: value,
            enable: enabledIf
        );
      }
    }
  }
}
