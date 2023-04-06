namespace WebApi.Controllers
{
    public class VoteRequest
    {
        public int storyId { set; get; }
        public string option { set; get; }
    }
}