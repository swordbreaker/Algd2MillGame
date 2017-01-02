using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MillGame.Models.Core.Actions;

namespace MillGame.Models.Core
{
    /**
     * A MaxNode is a child of a MinNode
     * 
     * @author christoph.stamm
     * @version 24.11.2009
     */
    public class MaxNode : GameNode
    {
    /**
	 * Create node with action
	 * @param a Action
	 */
    public MaxNode(Actions.Action a) : base(a)
    {
    }

    /**
	 * Create node with action and score
	 * @param a Action
	 * @param score Score
	 */
    public MaxNode(Actions.Action a, int score) : base(a, score)
    {
    }

    /**
	 * The children of a MinNode will be ordered in decreasing score order
	 */
    public int CompareTo(INode<Action> v)
    {
        int score1 = Score(), score2 = ((GameNode)v).Score();

        if (score1 == score2) return 0;
        else return (score1 > score2) ? -1 : 1;
    }

    /**
	 * Get winner score
	 */
    public int GetWinnerScore()
    {
        return State.WHITEWINS;
    }
}

}
