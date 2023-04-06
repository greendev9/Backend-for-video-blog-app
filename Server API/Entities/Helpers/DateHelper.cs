using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Helpers
{
    public static class DateHelper
    {
        public static string GetShowDate(this DateTime data)
        {
            if (DateTime.Today == data.Date)
                return "Today";
            if ((DateTime.Now - data.Date).TotalHours < 48)
                return "Yesterday";
            return data.ToString("dd-MM-YYYY");
        }
        public static int GetAge(this DateTime birthDate)
        {
            DateTime n = DateTime.Now; // To avoid a race condition around midnight
            int age = n.Year - birthDate.Year;

            if (n.Month < birthDate.Month || (n.Month == birthDate.Month && n.Day < birthDate.Day))
                age--;

            return age;
        }
        public static int GetAge(int birthyear)
        {
            if (birthyear < 1918)
                return 0;
            DateTime n = DateTime.Now; // To avoid a race condition around midnight
            int age = n.Year - birthyear;
            return age;
        }


        public static KeyValuePair<int, int> GetAgeGroup(int age)
        {
            Dictionary<int, int> ageGroups = new Dictionary<int, int>();
  		ageGroups.Add(1, 500);
       
		
            foreach (var item in ageGroups)
            {
                if (age >= item.Key && age <= item.Value)
                {
                    return item;
                }
            }
            return new KeyValuePair<int, int>(0, 0);
        }




        public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek)
        {
            int diff = (7 + (dt.DayOfWeek - startOfWeek)) % 7;
            return dt.AddDays(-1 * diff).Date;
        }
    }
}
