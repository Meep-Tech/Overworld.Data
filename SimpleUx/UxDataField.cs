using System;
using System.Collections.Generic;

namespace Overworld.Ux.Simple {

  /// <summary>
  /// A data field for input or display in a simple ux pannel/view
  /// </summary>
  public class UxDataField : IUxViewElement {

    /// <summary>
    /// The view this field is in.
    /// </summary>
    public UxView View {
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
    public Func<UxDataField, UxView, bool> Enable {
      get;
    }

    internal UxDataField _controllerField;

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
    public UxDataField(
      DisplayType type,
      string name,
      string tooltip = null,
      object value = null,
      string dataKey = null, 
      bool isReadOnly = false,
      Func<UxDataField, UxView, bool> enable = null,
      object validation = null
    ) {
      Type = type;
      Name = name;
      Tooltip = tooltip;
      Value = value;
      DataKey = string.IsNullOrWhiteSpace(dataKey)
        ? name
        : dataKey;
      IsReadOnly = isReadOnly;
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
          if(Validation.GetType() == typeof((float, float))) {
            double number = (double)value;
            (double min, double max) bounds = (((float, float))Validation);
            if(number > bounds.max && number < bounds.min) {
              message = "Number Out Of Range Bounds";
              return false;
            }
          }
          break;
        case DisplayType.Text:
          if(Validation.GetType() == typeof((float, float))) {
            double number = (double)value;
            (double min, double max) = (((float, float))Validation);
            if(number > max && number < min) {
              message = "Number Out Of Range Bounds";
              return false;
            }
          } else if(Validation.GetType() == typeof((int, int))) {
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

        (_controllerField as UxKeyValueSet)._update(pair);
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
    public UxDataField Copy(UxView toNewView = null) {
      var newField = MemberwiseClone() as UxDataField;
      newField.View = toNewView;

      return newField;
    }

    ///<summary><inheritdoc/></summary>
    IUxViewElement IUxViewElement.Copy(UxView toNewView)
      => Copy(toNewView);
  }
}
