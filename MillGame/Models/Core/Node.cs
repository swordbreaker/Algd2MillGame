using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MillGame.Models
{
    /**
     * 
     * @author Christoph Stamm
     * @version 16.7.2009
     *
     */

    /**
     * @author  christoph.stamm
     */
    public abstract class Node<T> : INode<T>, IComparable<Node<T>>, IEnumerable<Node<T>>
    {
        protected T m_data;                     // private data
        protected Node<T> m_parent;             // parent node
        internal PriorityQueue<Node<T>> m_children;    // children

        /**
         * Create leaf with private data
         * @param data Private data
         */
        public Node(T data)
        {
            m_data = data;
            m_children = new PriorityQueue<Node<T>>(Comparer<Node<T>>.Create((node, node1) => node.CompareTo(node1)));
        }

        public override String ToString()
        {
            return (m_data != null) ? m_data.ToString() : "__-__:__";
        }

        /**
         * @return Resulting position
         */
        public int Add(Node<T> v)
        {
            v.m_parent = this;
            m_children.Enqueue(v);
            return Size() - 1;
        }

        /**
         * Remove this node
         */
        public void Remove()
        {
            //TODO create Remove Methode in Priority Queue
            if (m_parent != null) m_parent.m_children.Remove(this);
        }

        /**
         * Return number of children
         * @return Number of children
         */
        public int Size()
        {
            return m_children.Count;
        }

        public T Data()
        {
            return m_data;
        }

        /**
         * Count number of nodes in this subtree
         * @return Number of nodes in this subtree
         */
        public int Count()
        {
            int c = 1;
            foreach (Node<T> v in m_children)
            {
                c += v.Count();
            }
            return c;
        }

        /**
         * This iterator allows iterating through all direct children of this node.
         */
        public IEnumerator<Node<T>> Iterator()
        {
            return (IEnumerator<Node<T>>)m_children.GetEnumerator();
        }

        public abstract int CompareTo(Node<T> other);

        public IEnumerator<Node<T>> GetEnumerator()
        {
            return Iterator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

}
