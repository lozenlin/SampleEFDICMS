using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Data.Domain.Model
{
    public class ArticleVideoForFrontend
    {
        public System.Guid VidId { get; set; }
        public string VidSubject { get; set; }
        public string VidDesc { get; set; }

        public Nullable<int> SortNo { get; set; }
        public string SourceVideoId { get; set; }
    }
}
