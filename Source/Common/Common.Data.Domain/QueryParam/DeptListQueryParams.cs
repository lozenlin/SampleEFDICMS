using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Data.Domain.QueryParam
{
    public class DeptListQueryParams
    {
        public string Kw = "";
        public PagedListQueryParams PagedParams;
        public AuthenticationQueryParams AuthParams;

        public DeptListQueryParams()
        {
            PagedParams = new PagedListQueryParams();
            AuthParams = new AuthenticationQueryParams();
        }
    }
}
