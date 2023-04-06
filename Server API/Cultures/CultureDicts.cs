using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cultures
{
    public static class CD
    {
        public static Dictionary<string, Culture> Cultures = new Dictionary<string, Culture>()
        {
            {"en",new Culture
            {
                Id = 1,
                IsImplemented = true,
                IsRTL = false,
                Money = "₪",
                Name = "en",
                SepStart = "<en>",
                SepEnd = "</en>",
                IsDefault = true
            }},
            {"he",new Culture
            {
                Id = 2,
                IsImplemented = true,
                IsRTL = true,
                Money = "₪",
                Name = "he",
                SepStart = "<he>",
                SepEnd = "</he>",
                IsDefault = false
            }},
            {"ru",new Culture
            {
                Id = 3,
                IsImplemented = true,
                IsRTL = true,
                Money = "₪",
                Name = "ru",
                SepStart = "<ru>",
                SepEnd = "</ru>",
                IsDefault = false
            }}
        };
        private static Culture DefCult;

        public static Culture DefaultCulture 
        {
            get 
            {
                if(DefCult == null)
                    DefCult = Cultures.First(p => p.Value.IsDefault).Value;
                return DefCult;
            }
        }

        public static Culture GetCulture(string cult="")
        {
            if(string.IsNullOrEmpty(cult))
            {
                return DefaultCulture;
            }
            else
            {
                Culture rv;
                if(Cultures.TryGetValue(cult,out rv))
                {
                    return rv;
                }
                else
                {
                    return DefaultCulture;
                }
            }
        }

        public static Culture GetCultureById(int cultid)
        {
            return Cultures.First(p => p.Value.Id == cultid).Value;
        }

        public static void SetThreadCulture(Culture c)
        {
            if (Thread.CurrentThread.CurrentCulture.Name != new System.Globalization.CultureInfo(c.Name).Name)
            {
                Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(c.Name);
                Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;
            }
        }

        public static string GetCulturedString(this string Data, string CultureName)
        {
            if (string.IsNullOrEmpty(Data))
                return "";
            int startIndex = Data.IndexOf(Cultures[CultureName].SepStart);
            if (startIndex >= 0)
            {
                int endIndex = Data.IndexOf(Cultures[CultureName].SepEnd);
                int start = startIndex + Cultures[CultureName].SepStart.Length;
                int end = endIndex - start;
                return Data.Substring(start, end);
            }
            else
            {
                return string.Empty;
            }
        }

        private static bool ContainsCultureSeperators(string data, string CultureName)
        {
            if (data.Contains(Cultures[CultureName].SepStart))
                return true;
            return false;
        }

        public static string GetDefaultCultureName()
        {
            return DefaultCulture.Name;
        }

        public static string SetCulturedString(string Data, string oldData, string CultureName)
        {
            var oldcstring = oldData.GetCulturedString(CultureName);
            if (string.IsNullOrEmpty(oldcstring) && !ContainsCultureSeperators(oldData, CultureName))
            {
                string toInsert = oldcstring.Insert(0, Cultures[CultureName].SepStart);
                toInsert = toInsert.Insert(toInsert.Length, Data);
                toInsert = toInsert.Insert(toInsert.Length, Cultures[CultureName].SepEnd);
                return oldData.Insert(oldData.Length, toInsert);
            }
            if (string.IsNullOrEmpty(oldcstring) && ContainsCultureSeperators(oldData, CultureName))
            {
                string toInsert = Cultures[CultureName].SepStart;
                toInsert += Data;
                toInsert += Cultures[CultureName].SepEnd;
                return oldData.Replace(Cultures[CultureName].SepStart + Cultures[CultureName].SepEnd, toInsert);
            }
            if (!string.IsNullOrEmpty(oldcstring) && ContainsCultureSeperators(oldData, CultureName))
            {
                string toInsert = Cultures[CultureName].SepStart;
                toInsert += Data;
                toInsert += Cultures[CultureName].SepEnd;
                return oldData.Replace(Cultures[CultureName].SepStart + oldcstring + Cultures[CultureName].SepEnd, toInsert);
            }
            if (!string.IsNullOrEmpty(oldcstring) && !ContainsCultureSeperators(oldData, CultureName))
            {
                string toInsert = oldcstring.Insert(0, Cultures[CultureName].SepStart);
                toInsert = toInsert.Insert(toInsert.Length, Data);
                toInsert = toInsert.Insert(toInsert.Length, Cultures[CultureName].SepEnd);
                return oldData.Insert(oldData.Length, toInsert);
            }
            return oldData.Replace(oldcstring, Data);
        }

    }
}
