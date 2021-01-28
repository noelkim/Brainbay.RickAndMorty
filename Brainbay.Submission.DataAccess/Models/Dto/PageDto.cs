using System.Collections.Generic;

namespace Brainbay.Submission.DataAccess.Models.Dto
{
    public class PageDto<T>
    {
        public PageInfoDto Info { get; set; }
        public IEnumerable<T> Results { get; set; }
    }
}
