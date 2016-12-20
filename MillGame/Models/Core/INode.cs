using System;
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

    public interface INode<T>
    {
        /**
         * Append child node at last position
         * @param v Node
         * @return Resulting position
         */
        int Add(Node<T> v);

        /**
         * Remove this node
         */
        void Remove();

        /**
         * Return number of children
         * @return Number of children
         */
        int Size();

        /**
         * Return node data
         * @return Node data
         */
        T Data();

        /**
         * Count number of nodes in this subtree
         * @return Number of nodes in this subtree
         */
        int Count();

        /**
         * This iterator allows iterating through all direct children of this node.
         */
        IEnumerator<Node<T>> Iterator();
    }

}
