using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace Domain.Helpers
{
  public static class ObjectExtensions
  {
    /// <summary>
    /// Unsafely casts the current object to the specifies type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static T ToType<T>(this object obj)
    {
      return (T)obj;
    }

    /// <summary>
    /// Safely casts the current object to the specifies type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static T SafeToType<T>(this object obj) where T : class
    {
      return obj as T;
    }

    /// <summary>
    /// Deep clones the current object using binary formatting.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static T DeepClone<T>(this T obj)
    {
      using (var ms = new MemoryStream())
      {
        var formatter = new BinaryFormatter();
        formatter.Serialize(ms, obj);
        ms.Position = 0;

        return (T)formatter.Deserialize(ms);
      }
    }


    /// <summary>
    /// Deep clones the current object to a similar type using xml serialization.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static TResult DeepCloneSimilar<TResult>(this object obj)
    {
      using (var ms = new MemoryStream())
      {
        var serializer = new XmlSerializer(typeof(TResult));
        serializer.Serialize(ms, obj);
        ms.Position = 0;

        return (TResult)serializer.Deserialize(ms);
      }
    }

    /// <summary>
    /// Deep clones the current object to a similar type using json serialization.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static TResult DeepCloneSimilarJson<TResult>(this object obj)
    {
      return JsonConvert.DeserializeObject<TResult>(JsonConvert.SerializeObject(obj));
    }


    public static int IntTryParse(this string valueNotNull)
    {
      int res;
      bool isOK = int.TryParse(valueNotNull, out res);

      return res;
    }
    public static int IntTryParse(this string valueNotNull, int defaultValue)
    {
      int res;
      bool isOK = int.TryParse(valueNotNull, out res);
      if (isOK)
      {
        return res;
      }
      else
      {
        return defaultValue;
      }
    }

    public static decimal DecimalTryParse(this string valueNotNull)
    {
      decimal res;
      bool isOK = decimal.TryParse(valueNotNull, out res);

      return res;
    }
    public static decimal DecimalTryParse(this string valueNotNull, decimal defaultValue)
    {
      decimal res;
      bool isOK = decimal.TryParse(valueNotNull, out res);
      if (isOK)
      {
        return res;
      }
      else
      {
        return defaultValue;
      }

    }

    public static double DoubleTryParse(this string valueNotNull)
    {
      double res;
      bool isOK = double.TryParse(valueNotNull, out res);

      return res;
    }
    public static double DoubleTryParse(this string valueNotNull, double defaultValue)
    {
      double res;
      bool isOK = double.TryParse(valueNotNull, out res);
      if (isOK)
      {
        return res;
      }
      else
      {
        return defaultValue;
      }

    }

    public static bool BoolTryParse(this string valueNotNull)
    {
      bool res;
      bool isOK = bool.TryParse(valueNotNull, out res);

      return res;
    }
    public static bool BoolTryParse(this string valueNotNull, bool defaultValue)
    {
      bool res;
      bool isOK = bool.TryParse(valueNotNull, out res);
      if (isOK)
      {
        return res;
      }
      else
      {
        return defaultValue;
      }
    }


    public static DateTime DateTimeTryParse(this string valueNotNull)
    {
      DateTime res;
      bool isOK = DateTime.TryParse(valueNotNull, out res);

      return res;
    }
    public static DateTime DateTimeTryParse(this string valueNotNull, DateTime defaultValue)
    {
      DateTime res;
      bool isOK = DateTime.TryParse(valueNotNull, out res);
      if (isOK)
      {
        return res;
      }
      else
      {
        return defaultValue;
      }
    }


    public static TimeSpan TimeSpanTryParse(this string valueNotNull)
    {
      TimeSpan res;
      bool isOK = TimeSpan.TryParse(valueNotNull, out res);

      return res;
    }
    public static TimeSpan TimeSpanTryParse(this string valueNotNull, TimeSpan defaultValue)
    {
      TimeSpan res;
      bool isOK = TimeSpan.TryParse(valueNotNull, out res);
      if (isOK)
      {
        return res;
      }
      else
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
    public static TEnum EnumTryParse<TEnum>(this string val) where TEnum : struct
    {
      TEnum result = default(TEnum); ;
      bool isOK = Enum.TryParse<TEnum>(val, true, out result);
      return result;
    }

    /// <summary>
    /// Note: ignores case.
    /// </summary>
    /// <typeparam name="TEnum"></typeparam>
    /// <param name="val"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public static TEnum EnumTryParse<TEnum>(this string val, TEnum defaultValue) where TEnum : struct
    {
      TEnum result;
      bool isOK = Enum.TryParse<TEnum>(val, true, out result);
      if (isOK)
      {
        return result;
      }
      else
      {
        return defaultValue;
      }
    }

    /// <summary>
    /// Splits the given string value to a comma-separated-value (csv) array.
    /// </summary>
    /// <param name="val"></param>
    /// <returns></returns>
    public static List<string> SplitToCsv(this string val, bool lowered = false, bool trimmed = true, char delimiter = ',')
    {
      IEnumerable<string> res = val.Split(new char[1] { delimiter }, StringSplitOptions.RemoveEmptyEntries).AsEnumerable();
      if (lowered)
      {
        res = res.Select(item => item.ToLower());
      }

      if (trimmed)
      {
        res = res.Select(item => item.Trim());
      }
      return res.ToList();
    }


    public static object FromJson(this string json)
    {
      return JsonConvert.DeserializeObject(json);
    }

    public static T FromJson<T>(this string json)
    {
      return JsonConvert.DeserializeObject<T>(json);
    }

    public static string ToJson(this object obj)
    {
      return JsonConvert.SerializeObject(obj);
    }

  }
}
