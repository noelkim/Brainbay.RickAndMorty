namespace Brainbay.Submission.DataAccess.Models.Dto
{
    public class PageInfoDto
    {
        public int Count { get; set; }
        public int Pages { get; set; }
        public string Next { get; set; }
        public string Prev { get; set; }
    }
}
