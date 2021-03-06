﻿using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MillGame.Models.Core.Actions;

namespace MillGame.Models.Core.Actions
{
    ///////////////////////////////////////////////////////////////////////////////
    /**
     * Abstract base class for placing and moving actions
     */
    public abstract class ActionPM : Action
    {
        protected byte m_to;    // game board position: 0..23

        /**
         * Constructor
         * @param color Color of the stone being placed or moved
         * @param to Target position
         */
        public ActionPM(sbyte color, int to) : base(color)
        {
            if (!(to >= 0 && to < State.NPOS)) throw new Exception("wrong board position");
            m_to = (byte)to;
        }

        /**
         * 
         * @return Target position
         */
        public byte EndPosition => m_to;
    }
}
