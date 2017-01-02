using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MillGame.Models.Core;
using MillGame.Models.Core.Actions;
using Action = MillGame.Models.Core.Actions.Action;

namespace MillGame.Models
{
    /**
 * 
 * @author christoph.stamm
 * @version 24.11.2009
 *
 */
    public interface IGameNode : INode<Action> {
    /**
	 * Create new node and add it to this as a child node.
	 * @param a Action
	 * @param score Score
	 */
    GameNode Add(Action a, int score);

    /**
	 * Create new node and add it to this as a child node.
	 * @param a Action
	 */
    GameNode Add(Action a);

    /**
	 * Create new nodes recursively.
	 * @param curHeight current subtree height
	 * @param height Subtree height
	 * @param color Color of next actions
	 * @param root Subtree root
	 * @param rootState Game state at root
	 * @return Number of created nodes
	 */
    int Create(int curHeight, int height, sbyte color, GameNode root, State rootState);

    /**
	 * Compute game state at this node
	 * @param s Game state at given node v
	 * @param v Game node v must be ancestor of this
	 * @return Game state at this node
	 */
    State ComputeState(State s, GameNode v);

    /**
	 * @return Score of a winner node
	 */
    int GetWinnerScore();

    /**
	 * @return Score of this node
	 */
    int Score();
}

}
