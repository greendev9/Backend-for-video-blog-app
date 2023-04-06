using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Models.Posts
{
    public enum DateFilterType
    {
        NONE = 0,
        TODAY = 1,
        WEEK = 2,
        MONTH = 3,
        YEAR = 4
    }
}
