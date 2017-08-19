using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyEngine
{
    public class Screen
    {
        public static bool lockCursor
        {
            set
            {
                EngineMain.instance.CursorVisible = value;
            }
            get
            {
                return EngineMain.instance.CursorVisible;
            }
        }
    }
}
