using System;
using System.Collections.Generic;
using TheArtOfDev.HtmlRenderer.Adapters;
using TheArtOfDev.HtmlRenderer.Adapters.Entities;
using TheArtOfDev.HtmlRenderer.Core.Dom;
using TheArtOfDev.HtmlRenderer.Core.Entities;
using TheArtOfDev.HtmlRenderer.Core.Handlers;
using TheArtOfDev.HtmlRenderer.Core.Parse;
using TheArtOfDev.HtmlRenderer.Core.Utils;

namespace TheArtOfDev.HtmlRenderer.Core
{
    public class DOMElement
    {
        private CssBox _element;
        private DOMStyle _style;
        internal DOMElement(CssBox element)
        {
            _element = element;
            _style = new DOMStyle(element);
        }

        public DOMStyle style
        {
            get { return _style; }
        }



        public string this[string attribute]
        {
            get { return _element.GetAttribute(attribute); }
            //set {_element.set , value); }
        }


        public string innerHTML
        {
            get { return ""; }
            set { }
        }

        public DOMElement parentElement
        {
            get { return null; }
        }

        public DOMElement firstChild
        {
            get { return null; }
        }
        public DOMElement lastChild
        {
            get { return null; }
        }

        public List<DOMElement> childNodes
        {
            get { return null; }
        }

        public bool hasChildNodes()
        {
            return false;
        }

        public void remove()
        {

        }


        public DOMElement appendChild(DOMElement child)
        {
            return null;
        }
        public DOMElement replaceChild(DOMElement newChild, DOMElement oldChild)
        {
            return null;
        }
        public DOMElement removeChild(DOMElement child)
        {
            return null;
        }

    }
}
