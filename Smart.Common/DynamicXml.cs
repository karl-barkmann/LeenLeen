using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Smart.Common
{
    public class DynamicXml : DynamicObject
    {
        private readonly XElement root;

        private DynamicXml(XElement root)
        {
            this.root = root;
        }

        public static DynamicXml Parse(string xmlString)
        {
            return new DynamicXml(XDocument.Parse(xmlString).Root);
        }

        public static DynamicXml Load(string filename)
        {
            return new DynamicXml(XDocument.Load(filename).Root);
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            var att = root.Attribute(binder.Name);
            if (att != null)
            {
                result = att.Value;
                return true;
            }

            var nodes = root.Elements(binder.Name);
            if (nodes.Count() > 1)
            {
                result = nodes.Select(n => new DynamicXml(n)).ToList();
                return true;
            }

            var node = root.Element(binder.Name);
            if (node != null)
            {
                if (node.HasElements)
                {
                    result = new DynamicXml(node);
                }
                else if (node.HasAttributes)
                {
                    result = new DynamicXml(node);
                }
                else
                {
                    result = node.Value;
                }
                return true;
            }

            var elments = root.Descendants(binder.Name);
            if (elments.Any())
            {
                if (elments.Count() > 1)
                {
                    result = nodes.Select(n => new DynamicXml(n)).ToList();
                    return true;
                }
                if (elments.First().HasElements)
                {
                    result = new DynamicXml(elments.First());
                }
                else
                {
                    result = elments.First().Value;
                }

                return true;
            }

            result = null;
            return false;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            var att = root.Attribute(binder.Name);
            if (att != null)
            {
                att.SetValue(value);
                return true;
            }

            var nodes = root.Elements(binder.Name);
            if (nodes.Count() > 1)
            {
                return true;
            }

            var node = root.Element(binder.Name);
            if (node != null)
            {
                if (node.HasElements)
                {

                }
                else
                {
                    node.SetValue(value);
                }
                return true;
            }

            var elments = root.Descendants(binder.Name);
            if (!elments.Any()) return false;
            if (elments.Count() > 1)
            {
                return true;
            }
            if (elments.First().HasElements)
            {

            }
            else
            {
                elments.First().SetValue(value);
            }
            return true;
        }

        public new string ToString(string format = null)
        {
            if (format == "{iv}")
            {
                return root != null ? root.Value : String.Empty;
            }

            return root != null ? root.ToString() : base.ToString();
        }
    }

}
