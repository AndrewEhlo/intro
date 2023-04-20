using System.Linq;
using System.Text.RegularExpressions;
using VirtoCommerce.Storefront.Model;

namespace VirtoCommerce.LiquidThemeEngine.PostRenderTemplateChange.Operations
{
    public class ExternalLinksPostRenderTemplateOperation : IPostRenderTemplateChangeOperation
    {
        private readonly IWorkContextAccessor _workContextAccessor;

        public ExternalLinksPostRenderTemplateOperation(IWorkContextAccessor workContextAccessor, string[] urlWhitelist)
        {
            _workContextAccessor = workContextAccessor;
            if (urlWhitelist != null)
            {
                _urlWhitelist = urlWhitelist.Select(u => u.ToUpper()).ToArray();
            }
        }

        private readonly Regex _linksTagsRegex = new Regex(@"<\s*a[^>](.*?)>", RegexOptions.Compiled);
        private readonly Regex _hrefAttrRegex = new Regex(@"(?<=\bhref\s*=\s*[""'])[^""']*", RegexOptions.Compiled);
        private readonly string[] _urlWhitelist = new string[0];

        public string Run(string renderResult)
        {
            var matches = _linksTagsRegex.Matches(renderResult).Where(m => m.Success)
                .Select(m => m.Value)
                .Where(v => !string.IsNullOrEmpty(v)).ToList();

            foreach (var match in matches)
            {
                var hrefAttrValue = _hrefAttrRegex.Match(match).Value.Trim().ToUpper();
                var isHrefStartsWithCurrentStoreUrl = hrefAttrValue.StartsWith(_workContextAccessor.WorkContext.CurrentStore.Url.ToUpper());
                if ((hrefAttrValue.StartsWith("HTTP:") || hrefAttrValue.StartsWith("HTTPS:")) && !isHrefStartsWithCurrentStoreUrl)
                {
                    var matchWithRel = string.Empty;
                    if (_urlWhitelist.Any(u => hrefAttrValue.StartsWith(u)))
                    {
                        matchWithRel = match.Replace("<a", "<a rel=\"dofollow\" target=\"_blank\"");
                    }
                    else
                    {
                        matchWithRel = match.Replace("<a", "<a rel=\"nofollow\" target=\"_blank\"");
                    }
                    renderResult = renderResult.Replace(match, matchWithRel);
                }
            }
            return renderResult;
        }
    }
}
