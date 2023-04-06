using Entities.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Entities
{
    public class Word : IEntityBase
    {
        public int ID { get; set; }
        public string Value { get; set; }
    }
}
