using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Entities
{
  public class Comment : IEntityBase
  {
    public int ID { get; set; }
    public DateTime PublishDate { get; set; }
    [StringLength(600)]
    public string Body { get; set; }
    public User User { get; set; }
    public int UserId { get; set; }
    public Story Story { get; set; }
    public int StoryId { get; set; }
    [StringLength(6)]
    public string Language { get; set; }
    public bool? IsPendingApproval { get; set; }
    public bool? IsDeniedApproval { get; set; }
  }
}
