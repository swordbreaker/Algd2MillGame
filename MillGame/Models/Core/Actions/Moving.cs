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
     * Moving action
     */
    public class Moving : ActionPM
    {
        protected byte m_from;  // game board position: 0..23

        /**
         * Constructor
         * @param color Color of the stone being moved
         * @param from Start position
         * @param to Target position
         */
        public Moving(sbyte color, int from, int to) : base(color, to)
        {
            Contract.Assert(from >= 0 && from < State.NPOS, "wrong board position");
            Contract.Assert(from >= 0 && from < State.NPOS, "wrong board position");
            m_from = (byte)from;
        }


        public override bool Equals(object o)
        {
            if (o != null && o is Moving)
            {
                Moving a = (Moving)o;

                return a.m_color == m_color && a.m_to == m_to && a.m_from == m_from;
            }
            return false;
        }

        public override string ToString()
        {
            return string.Format("$0 ", 5);
            return string.Format("%02d-%02d:__", m_from, m_to);
            //return String.format("%02d-%02d:__", m_from, m_to);
        }

        /**
         * @return Start position
         */
        public byte StartPosition => m_from;

        public override bool IsValid(State s)
        {
            Contract.Assert(s != null, "s is null");
            return s.IsValidMove(m_from, m_to, m_color);
        }

        public override void Update(State s)
        {
            Contract.Assert(s != null, "s is null");
            s.update(this);
        }

        public override void Writeln(Stream os)
        {
            using (var writer = new StreamWriter(os, Encoding.ASCII, 1024, true))
            {
                writer.WriteLine("MOVE " + m_color + " " + m_from + " " + m_to);
            }

            //os.writeBytes("MOVE " + m_color + " " + m_from + " " + m_to + '\n');
        }
    }
}
