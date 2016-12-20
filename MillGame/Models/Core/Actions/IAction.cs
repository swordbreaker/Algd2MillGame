using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MillGame.Models.Core.Actions
{

    /**
     * 
     * @author Christoph Stamm
     * @version 16.7.2009
     * Converted to C# by Tobias Bollinger
     */
    public interface IAction
    {
        /**
         * Checks if this action is available at given game state
         * @param s Game state
         * @return True if this action is available
         */
        bool IsValid(State s);

        /**
         * Updates given game state with this action
         * @param s Game state
         */
        void Update(State s);

        /**
         * Writes action to data output stream
         * @param os Data output stream
         */
        void Writeln(Stream os);

        /**
         * Color of stone in action: WHITE, BLACK
         * @return WHITE or BLACK
         */
        sbyte Color();
    }
}


