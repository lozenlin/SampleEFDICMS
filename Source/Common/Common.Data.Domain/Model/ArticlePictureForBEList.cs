using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Data.Domain.Model
{
    public class ArticlePictureForBEList
    {
        public System.Guid PicId { get; set; }
        public string PicSubject { get; set; }
        public string PostAccount { get; set; }
        public Nullable<System.DateTime> PostDate { get; set; }
        public string MdfAccount { get; set; }
        public Nullable<System.DateTime> MdfDate { get; set; }

        public string FileSavedName { get; set; }
        public int FileSize { get; set; }
        public Nullable<int> SortNo { get; set; }
        public string FileMIME { get; set; }

        public bool IsShowInLangZhTw { get; set; }
        public bool IsShowInLangEn { get; set; }
        public int PostDeptId { get; set; }
        public string PostDeptName { get; set; }
        public int RowNum { get; set; }
    }
}
