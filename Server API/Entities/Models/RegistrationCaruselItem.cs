using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Entities
{
    public class RegistrationCaruselItem:IEntityBase
    {
        public int ID { get; set; }
        [StringLength(800)]
        public string Text { get; set; }
        [StringLength(100)]
        public string ImageName { get; set; }
        public int Index { get; set; }
    }
}
