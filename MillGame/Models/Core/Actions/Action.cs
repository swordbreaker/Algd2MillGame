using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MillGame.Models.Core.Actions;

namespace MillGame.Models.Core.Actions
{
    /**
     * An action represents a game state change. There are three different actions:
     * - placing a stone: a new stone is placed on the game board
     * - moving a stone: an existing stone is moved on the game board
     * - taking a stone: an existing stone is removed
     *  
     * @author Christoph Stamm
     * @version 16.7.2009
     *
     */
    public abstract class Action : IAction
    {
        protected sbyte m_color;     // color of stone in action: WHITE, BLACK

        public Action()
        {
            m_color = IController.NONE;
        }

        public Action(sbyte color)
        {
            if(!(color == IController.WHITE || color == IController.BLACK)) throw new Exception("wrong color");
            m_color = color;
        }

        /**
         * @return Color of the stone be placed or moved
         */
        public sbyte Color() => m_color;

        public static Action Readln(String[] token)
        {
            if (token[0].Equals("PLACE"))
            {
                // my opponent placed a stone
                if(token.Length != 3) throw new Exception();
                var color = sbyte.Parse(token[1]);
                int pos = int.Parse(token[2]);
                // create place action and play it
                return new Placing(color, pos);

            }
            else if (token[0].Equals("MOVE"))
            {
                // my opponent moved a stone
                if (token.Length != 4) throw new Exception();
                sbyte color = sbyte.Parse(token[1]);
                int from = int.Parse(token[2]);
                int to = int.Parse(token[3]);
                // create move action and play it
                return new Moving(color, from, to);

            }
            else if (token[0].Equals("TAKE"))
            {
                // my opponent 
                if (token.Length <= 1) throw new Exception();
                if (token[1].Equals("PLACE"))
                {
                    // my opponent placed a stone
                    if (token.Length != 5) throw new Exception();
                    sbyte color = sbyte.Parse(token[2]);
                    int pos = int.Parse(token[3]);
                    // create place action
                    ActionPM a = new Placing(color, pos);

                    int takepos = int.Parse(token[4]);
                    // create take and play it
                    return new Taking(a, takepos);

                }
                else if (token[1].Equals("MOVE"))
                {
                    // my opponent moved a stone
                    if (token.Length != 6) throw new Exception();
                    sbyte color = sbyte.Parse(token[2]);
                    int from = int.Parse(token[3]);
                    int to = int.Parse(token[4]);
                    // create move action
                    ActionPM a = new Moving(color, from, to);

                    int takepos = int.Parse(token[5]);
                    // create take and play it
                    return new Taking(a, takepos);

                }
            }
            return null;
        }

        public abstract bool IsValid(State s);
        public abstract void Update(State s);
        public abstract void Writeln(Stream os);
    }
}



