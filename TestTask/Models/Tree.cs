using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestTask.Models
{
    public class Tree<T>
    {
        public Node<T> HeadNode { get; set; }
    }

    public class Node<T>
    {
        public T Value { get; set; }

        public Node<T> ParentNode { get; set; }

        public ICollection<Node<T>> SubNodes { get; set; }
    }
}
