using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MillGame.Models.Core.Actions
{
    ///////////////////////////////////////////////////////////////////////////////
    /**
     * Taking action
     * @author  christoph.stamm
     */
    public class Taking : Action
    {
        /**
         * @uml.property  name="m_action"
         * @uml.associationEnd  
         */

        /***
     * Constructor
     * @param action Immediate action before this taking action
     * @param pos Position of the stone to be taken
     */
        public Taking(ActionPM action, int pos) : base(action.Color())
        {
            if (!(action != null && (action is Placing || action is Moving))) throw new Exception("wrong action");
            if (!(pos >= 0 && pos < State.NPOS)) throw new Exception("wrong board position");

            Action = action; // the action resulting in a take
            TakePosition = (byte)pos;
        }

        public override bool Equals(object o)
        {
            if (o != null && o is Taking)
            {
                var a = (Taking)o;
                return a.m_color == m_color && a.TakePosition == TakePosition && a.Action.Equals(Action);
            }
            return false;
        }


        public override string ToString()
        {
            if (Action is Moving)
            {
                return $"TAKING MOVING {((Moving) Action).StartPosition,2}-{Action.EndPosition,2}:{TakePosition,2}";
            }
            else
            {
                return $"TAKING __-{Action.EndPosition,2}:{TakePosition,2}";
            }
            //return "PSST nothing to see here";
        }

        /**
         * @return Position of the stone to be taken
         */
        public byte TakePosition { get; }

        public sbyte TakeColor => State.OppositeColor(m_color);

        /**
         * @return Immediate action before this taking action
         */
        public ActionPM Action { get; }

        public override bool IsValid(State s)
        {
            if (s == null) throw new Exception("s is null");
            return s.IsValidTake(TakePosition, State.OppositeColor(m_color));
        }

        public override void Update(State s)
        {
            if (s == null) throw new Exception("s is null");
            var moving = Action as Moving;
            if (moving != null)
            {
                s.Update(moving);
            }
            else
            {
                s.Update((Placing)Action);
            }
            s.Update(this);
        }

        public override void Writeln(Stream os)
        {
            using (var writer = new StreamWriter(os, Encoding.ASCII, 1024, true))
            {
                var moving = Action as Moving;
                if (moving != null)
                {
                    writer.WriteLine("TAKE MOVE " + m_color + " " + moving.StartPosition + " " + moving.EndPosition + " " + TakePosition);
                }
                else
                {
                    writer.WriteLine("TAKE PLACE " + m_color + " " + Action.EndPosition + " " + TakePosition);
                }
            }
        }
    }
}
