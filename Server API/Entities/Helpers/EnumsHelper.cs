using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace Domain.Helpers
{
  public static class EnumsHelper
  {
    public static string GetDescriptionFromEnumValue(Enum EnumValue)    //Note: EnumMeber overload.
    {
      EnumMemberAttribute attribute = EnumValue.GetType()
          .GetField(EnumValue.ToString())
          .GetCustomAttributes(typeof(EnumMemberAttribute), false)
          .SingleOrDefault() as EnumMemberAttribute;
      return attribute == null ? EnumValue.ToString() : attribute.Value;
    }

    public static string GetEnumDescription(Enum value)    //Note: Description overload.
    {
      FieldInfo fi = value.GetType().GetField(value.ToString());

      DescriptionAttribute[] attributes =
          (DescriptionAttribute[])fi.GetCustomAttributes(
          typeof(DescriptionAttribute),
          false);

      if (attributes.Length > 0)
        return attributes.Last().Description;
      else
        return value.ToString();
    }

    public static string GetEnumDescription(Enum value, string context)    //Note: ContextDescription overload.
    {
      if (string.IsNullOrEmpty(context))   //return description overload.
      {
        return GetEnumDescription(value);
      }

      FieldInfo fi = value.GetType().GetField(value.ToString());

      ContextDescriptionAttribute[] attributes =
          (ContextDescriptionAttribute[])fi.GetCustomAttributes(
          typeof(ContextDescriptionAttribute),
          false);

      ContextDescriptionAttribute att = null;
      if (attributes.Length > 0 && (att = attributes.FirstOrDefault(item => item.Context == context)) != null)
      {
        return att.Description;
      }
      else
      {
        return value.ToString();
      }
    }

    public static string GetNameOrDefault(Type enumType, object value, string defaultValue = "")
    {
      try
      {
        if (value == null)
        {
          return defaultValue;
        }

        return Enum.GetName(enumType, value) ?? defaultValue;
      }
      catch (Exception)
      {
        return defaultValue;
      }
    }

    /// <summary>
    /// Note: ignores case.
    /// </summary>
    /// <typeparam name="TEnum"></typeparam>
    /// <param name="val"></param>
    /// <returns></returns>
    public static TEnum ParseEnum<TEnum>(string val) where TEnum : struct
    {
      return ParseEnum<TEnum>(val, true);
    }

    public static TEnum ParseEnum<TEnum>(string val, bool ignoreCase) where TEnum : struct
    {
      return (TEnum)Enum.Parse(typeof(TEnum), val, ignoreCase);
    }

    /// <summary>
    /// Note: ignores case.
    /// </summary>
    /// <typeparam name="TEnum"></typeparam>
    /// <param name="val"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public static TEnum ParseEnum<TEnum>(string val, TEnum defaultValue) where TEnum : struct
    {
      return ParseEnum<TEnum>(val, defaultValue, true);
    }

    public static TEnum ParseEnum<TEnum>(string val, TEnum defaultValue, bool ignoreCase) where TEnum : struct
    {
      TEnum result;
      bool isOK = Enum.TryParse<TEnum>(val, ignoreCase, out result);
      if (isOK)
      {
        return result;
      }
      else
      {
        return defaultValue;
      }
    }

    public static bool IsAnyOf(this Enum obj, params Enum[] options)
    {
      foreach (var option in options)
      {
        if (Equals(obj, option))
        {
          return true;
        }
      }
      return false;
    }

    public static string ToDescriptionString(this Enum obj)    //description overload.
    {
      return GetEnumDescription(obj);
    }

    public static string ToDescriptionString(this Enum obj, string context)    //contextdescription overload
    {
      return GetEnumDescription(obj, context);
    }
  }

  /// <summary>
  /// Allows to specify a description for a particular context.
  /// </summary>
  [AttributeUsage(AttributeTargets.All)]
  public class ContextDescriptionAttribute : DescriptionAttribute
  {
    /// <summary>
    /// Holds the context to which the attribute applies.
    /// Note: null for default.
    /// </summary>
    public string Context { get; set; }

    public new string Description   //Note: Intentional hide.
    {
      get { return base.Description; }
      set { DescriptionValue = value; }
    }

    public ContextDescriptionAttribute()
        : base()
    {
    }
    public ContextDescriptionAttribute(string description)
        : base(description)
    {
    }
    public ContextDescriptionAttribute(string context, string description)
        : base(description)
    {
      this.Context = context;
    }
  }
}
