using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MillGame.Models.Core
{
    /**
     * 
     * @author christoph.stamm
     * @version  14.9.2010
     *
     */
    public class ComputerPlayer
    {
        private IController m_controller;
        private bool m_signal;
        private IController.Status m_status;
        private object _lock = new object();
        private Semaphore _mutex = new Semaphore(0, 1);

        public ComputerPlayer(IController controller)
        {
            m_controller = controller;
        }

        public void Finish()
        {
            Debug.WriteLine("FINISH COMPUTER PLAYER");
            m_status = IController.Status.FINISHED;
            m_signal = true;
        }

        public void Play()
        {
            Task.Run(() =>
            {
                m_controller.Compute();
                m_status = m_controller.GetStatus();
            });
        }
    }
}