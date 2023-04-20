using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using VirtoCommerce.LiquidThemeEngine.PostRenderTemplateChange.Operations;
using VirtoCommerce.Storefront.Model;

namespace VirtoCommerce.LiquidThemeEngine.PostRenderTemplateChange
{
    public class PostRenderTemplateChanger : IPostRenderTemplateChange
    {
        private readonly IList<IPostRenderTemplateChangeOperation> _operations;

        public PostRenderTemplateChanger(IWorkContextAccessor workContextAccessor, IConfiguration postRenderTemplateChangeConfig)
        {
            var urlWhitelist = postRenderTemplateChangeConfig.GetSection("ExternalLinks:UrlWhitelist").Get<string[]>();
            _operations = new[]
            {
                new ExternalLinksPostRenderTemplateOperation(workContextAccessor, urlWhitelist)
            };
        }

        public IList<IPostRenderTemplateChangeOperation> Operations => _operations;

        public string Change(string renderResult)
        {
            foreach (var operation in Operations)
            {
                renderResult = operation.Run(renderResult);
            }
            return renderResult;
        }
    }
}
