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
     * Placing action
     */
    public class Placing : ActionPM
    {
        /**
         * Constructor
         * @param color Color of the stone being placed
         * @param to Target position
         */
        public Placing(sbyte color, int to) : base(color, to) { }

        public override bool Equals(Object o)
        {
            if (o != null && o is Placing)
            {
                Placing a = (Placing)o;

                return a.m_color == m_color && a.m_to == m_to;
            }
            return false;
        }

        //public static bool operator ==(Placing p1, Placing p2)
        //{
        //    return p1 != null && p1.Equals(p2);
        //}

        //public static bool operator !=(Placing p1, Placing p2)
        //{
        //    return !(p1 == p2);
        //}

        public override string ToString()
        {
            //TODO Fix
            return $"__-{m_to,2}:__";
            //return String.format("__-%02d:__", m_to);
        }

        public override bool IsValid(State s)
        {
            Contract.Assert(s != null, "s is null");
            return s.IsValidPlace(m_to, m_color);
        }

        public override void Update(State s)
        {
            Contract.Assert(s != null, "s is null");
            s.Update(this);
        }

        public override void Writeln(Stream os)
        {
            using (var writer = new StreamWriter(os, Encoding.ASCII, 1024, true))
            {
                writer.WriteLine("PLACE " + m_color + " " + m_to);
            }

            //os.writeBytes("PLACE " + m_color + " " + m_to + '\n');
        }
    }
}
