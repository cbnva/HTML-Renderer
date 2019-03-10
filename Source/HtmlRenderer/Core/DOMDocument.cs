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
    public class DOMDocument
    {

        private CssBox _root = null;

        internal DOMDocument(CssBox root)
        {
            _root = root;
        }
        /// <summary>
        /// Find an element by element id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public DOMElement getElementById(string id)
        {
            CssBox element = DomUtils.GetBoxById(_root, id);
            if (element == null)
                return null;
            else
                return new DOMElement(element);
        }
        /// <summary>
        /// Find elements by tag name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public List<DOMElement> getElementsByTagName(string name)
        {
            return null;
        }

        /// <summary>
        /// Find elements by class name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public List<DOMElement> getElementsByClassName(string name)
        {
            return null;
        }


        public DOMElement createElement(string tagName)
        {
            return null;
        }

        public DOMElement body
        {
            get { return null; }
        }


    }







}
