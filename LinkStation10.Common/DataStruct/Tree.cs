using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMEGo.Sunflower.LinkStation10.Common
{
    public class Node<T>
    {
        [JsonIgnore]
        public Node<T> SuperNode;
        public IList<Node<T>> SubNode;
        public string Name;
        public T Value;
        public Node<T> this[int i] {
            get {
                if (this.SubNode == null || this.SubNode.Count() < i || i < 0)
                {
                    return null;
                }
                return this.SubNode[i];
            }
            set {
                if (this.SubNode == null || this.SubNode.Count() < i || i < 0)
                {
                    return;
                }
                this.SubNode[i] = value;
                value.SuperNode = this;
            }
        }
        public Node<T> this[string name] {
            get {
                if (this.SubNode == null || string.IsNullOrEmpty(name))
                {
                    return null;
                }
                var result = from e in this.SubNode where e.Name.Equals(name, StringComparison.OrdinalIgnoreCase) select e;
                if (result.Count() > 0)
                {
                    return result.First();
                }
                return null;
            }
            set {
                if (this.SubNode == null || string.IsNullOrEmpty(name))
                {
                    return;
                }
                var result = from e in this.SubNode where e.Name.Equals(name, StringComparison.OrdinalIgnoreCase) select e;
                if (result.Count() > 0)
                {
                    var node = result.First();
                    var i = this.SubNode.IndexOf(node);
                    this.SubNode[i] = value;
                    value.SuperNode = this;
                }
            }
        }
        public IEnumerable<Node<T>> DeepestNodes()
        {
            var list = new List<Node<T>>();
            if (this.SubNode == null || this.SubNode.Count < 1)
            {
                list.Add(this);
            }
            else
            {
                foreach (var node in this.SubNode)
                {
                    list.AddRange(node.DeepestNodes());
                }
            }
            return list;
        }
        public IEnumerable<Node<T>> Descendant()
        {
            var list = new List<Node<T>>
            {
                this
            };
            if (this.SubNode != null)
            {
                foreach (var node in this.SubNode)
                {
                    list.AddRange(node.Descendant());
                }
            }
            return list;
        }
        public Node (string name, T value)
        {
            this.Name = name;
            this.Value = value;
            this.SuperNode = null;
            this.SubNode = new List<Node<T>>();
        }
        public Node() { }
        public static Node<T> Empty {
            get {
                return new Node<T>
                {
                    SuperNode = null,
                    SubNode = Array.Empty<Node<T>>().ToList(),
                    Name = "",
                    Value = default(T)
                };
            }
        }
    }
}
