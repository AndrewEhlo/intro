using System;
using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.Storefront.Model.StaticContent;

namespace VirtoCommerce.LiquidThemeEngine.Filters
{
    public static partial class ArticlesFilters
    {
        public static object ByCategory(object input, string category)
        {
            if (input is IEnumerable<BlogArticle> articles)
            {
                return articles.Where(a => a.Category != null && (a.Category == category || a.Category.Split(';', StringSplitOptions.None).Contains(category))).ToList();
            }
            return input;
        }
    }
}
