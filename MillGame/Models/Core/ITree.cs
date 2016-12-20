namespace MillGame.Models.Core
{
    /**
     * 
     * @author Christoph Stamm
     * @version 16.7.2009
     *
     */

    public interface ITree
    {
        /**
         * Visualizes this tree on system console.
         */
        void Print();

        /**
         * @return Number of nodes in this tree.
         */
        int Size();
    }

}
