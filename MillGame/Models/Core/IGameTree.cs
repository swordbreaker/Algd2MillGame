using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MillGame.Models.Core.Actions;
using Action = System.Action;

namespace MillGame.Models.Core
{
    /**
     * Black is a minimum player (minimizer); White is a maximum player (maximizer).
     * 
     * @author christoph.stamm
     * @version 14.9.2010
     *
     */
    public interface IGameTree : ITree
    {
    /**
	 * Creates a new game tree: the first action is white, on the next level plays black.
	 * White is a maximizer, black is a minimizer. 
	 * @param pa null if computer plays white, first action if human plays white
	 */
    void Create(int height, Placing pa);

    /**
	 * Return current game state.
	 * @return Current game state.
	 */
    State CurrentState();

    /**
	 * Update tree (remove subtrees), current node, and current state for the human player
	 * @param a Action
	 */
    void HumanPlayer(Actions.Action a);

        /**
         * Compute best next node at current node, update tree (remove subtrees), current node, 
         * and current state for the computer player
         * @return Best action or null
         */
        Actions.Action ComputerPlayer();
}

}
