﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.DataAccess.EF
{
    public class ArticleValidListQueryParamsDA
    {
        public Guid ParentId;
        public string CultureName;
        public string Kw;
        public PagedListQueryParamsDA PagedParams;

        public ArticleValidListQueryParamsDA()
        {
            PagedParams = new PagedListQueryParamsDA();
        }
    }
}