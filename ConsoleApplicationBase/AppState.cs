using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplicationBase
{
    public static class AppState
    {
        static State _state;

        static AppState()
        {
            _state = State.IDLE;
        }

        public static State GetState()
        {
            return _state;
        }

        public static void SetState(State newState)
        {
            _state = newState;
        }

    }

    public enum State
    {
        ERROR = -1,
        IDLE = 0,
        RUNNING = 1
    }
}
