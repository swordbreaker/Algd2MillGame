using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MillGame.Models.Core;
using MillGame.Models.Core.Actions;

namespace MillGame.Models
{
    /**
     * A MinNode is a child of a MaxNode
     * 
     * @author christoph.stamm
     * @version 24.11.2009
     *
     */
    public class MinNode : GameNode
    {
    /**
	 * Create node with action
	 * @param a Action
	 */
    public MinNode(Action a) : base(a)
    {
    }

    /**
	 * Create node with action and score
	 * @param a Action
	 * @param score Score
	 */
    public MinNode(Action a, int score) : base(a, score) { }

    /**
	 * The children of a MaxNode will be ordered in increasing score order
	 */
    public int CompareTo(INode<Action> v)
    {
        int score2 = ((GameNode)v).Score();

        if (m_score == score2) return 0;
        else return (m_score < score2) ? -1 : 1;
    }

    /**
	 * Get winner score
	 */
    public int GetWinnerScore()
    {
        return State.BLACKWINS;
    }
}

}
