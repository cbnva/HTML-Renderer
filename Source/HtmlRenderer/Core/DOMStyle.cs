using System;
using TheArtOfDev.HtmlRenderer.Adapters;
using TheArtOfDev.HtmlRenderer.Adapters.Entities;
using TheArtOfDev.HtmlRenderer.Core.Dom;
using TheArtOfDev.HtmlRenderer.Core.Entities;
using TheArtOfDev.HtmlRenderer.Core.Handlers;
using TheArtOfDev.HtmlRenderer.Core.Parse;
using TheArtOfDev.HtmlRenderer.Core.Utils;

namespace TheArtOfDev.HtmlRenderer.Core
{
    public class DOMStyle
    {
        private CssBox _element;
        internal DOMStyle(CssBox element)
        {
            _element = element;
        }

        public string this[string property]
        {
            get { return CssUtils.GetPropertyValue(_element, property); }
            set { CssUtils.SetPropertyValue(_element, property, value); }
        }


    }
}
