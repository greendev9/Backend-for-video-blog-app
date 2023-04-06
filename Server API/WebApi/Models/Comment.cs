using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Models
{
    public class CommentModel
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "Requiredfield")]
        //[StringLength(600, ErrorMessage = "CommentBodyLengh")]
        public string Body { get; set; }

        [Required] 
        public int StoryId { get; set; }
        
        public string Language { get; set; }

        public bool IsPending { get; set; }
    }

    //public class CommentModel
    //{
    //    public int ID { get; set; }
    //    public DateTime PublishDate { get; set; }
    //    public string Body { get; set; }
    //    public int UserId { get; set; }
    //    public int StoryId { get; set; }
    //    public string Language { get; set; }
    //    public bool? IsPendingApproval { get; set; }
    //    public bool? IsDeniedApproval { get; set; }
    //}
}
