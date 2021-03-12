using Common.LogicObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JsonService
{
    public static class JsonServiceHandlerFactory
    {
        public static IJsonServiceHandler GetHandler(HttpContext context, string serviceName, OtherArticlePageCommon c, ArticlePublisherLogic artPub)
        {
            IJsonServiceHandler handler = null;

            switch (serviceName)
            {
                case "Article_GetListWithThumb":
                    handler = new Article_GetListWithThumb(context, c, artPub);
                    break;
                case "Keyword_GetList":
                    handler = new Keyword_GetList(context, c, artPub);
                    break;
            }

            return handler;
        }
    }
}