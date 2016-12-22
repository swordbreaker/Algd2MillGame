using System;
using System.Collections.Generic;
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
        private Mutex _mutex = new Mutex();

        public ComputerPlayer(IController controller)
        {
            m_controller = controller;
        }

        public void Finish()
        {
            m_status = IController.Status.FINISHED;
            m_signal = true;
            _mutex.ReleaseMutex();
            //notify();
        }

        public void Play()
        {
            m_signal = true;
            _mutex.ReleaseMutex();
            //notify();
        }

        public void Run()
        {
            lock (_lock)
            {
                do
                {
                    while (!m_signal) _mutex.WaitOne();
                    m_signal = false;
                    if (m_status != IController.Status.FINISHED)
                    {
                        Thread.Sleep(500);
                        m_controller.Compute();
                        m_status = m_controller.GetStatus();
                    }
                } while (m_status == IController.Status.OK || m_status == IController.Status.CLOSEDMILL);
            }
        }
    }
}