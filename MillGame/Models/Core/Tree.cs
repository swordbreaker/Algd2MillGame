using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MillGame.Models
{
    class Tree<T> : Core.ITree
    {
        /**
             * @uml.property  name="m_root"
             * @uml.associationEnd  
             */
        protected Node<T> m_root;   // root node
        protected int m_size;       // number of nodes

        /**
         * Standard constructor creates empty tree.
         */
        public Tree() { }

        /**
         * Create new tree with given node as root
         * @param root Root node
         */
        public Tree(Node<T> root)
        {
            m_root = root;
        }

        /**
         * @return Number of nodes in this tree
         */
        public int Size()
        {
            return m_size;
        }

        /**
         * Simple console tree printing
         */
        public void Print()
        {
            Console.WriteLine("Tree");
            if (m_root != null)
            {
                Print(m_root, 0, 0);
            }
            else
            {
                Console.WriteLine("Tree is empty");
            }
        }

        /**
         * Recursive pre-order tree printing
         * @param p Tree node
         * @param level Tree level: used as x-coordinate
         * @param childNumber Child ordering number: used as y-coordinate
         */
        private void Print(Node<T> p, int level, int childNumber)
        {
            Debug.Assert(p != null);

            if (childNumber > 0)
            {
                for (int i = 0; i < level; i++)
                {
                    Console.Write("\t" + "       |");
                }
            }
            Console.Write(p);
            Console.Write('\t');
            
            if (p.m_children.Count > 0)
            {
                int n = 0;
                foreach (Node<T> v in p.m_children)
                {
                    Print(v, level + 1, n++);
                }
                for (int i = 0; i < level; i++)
                {
                    Console.Write("\t" + "       |");
                }
                Console.WriteLine();
            }
            else
            {
                Console.WriteLine();
            }
        }
    }
}
