using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Data.Domain.EntityRequiredPropValues
{
    public class ArticleMultiLangRequiredPropValues
    {
        public System.Guid ArticleId { get; set; }
        public string CultureName { get; set; }
        public bool IsShowInLang { get; set; }
    }
}
