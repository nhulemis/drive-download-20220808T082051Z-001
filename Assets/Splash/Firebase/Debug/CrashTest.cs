using System;
using MobileConsole;
using UnityEngine.Diagnostics;

namespace Splash.Firebase.Debug
{
    
    [ExecutableCommand(name = "Firebase/Crash Test")]
    public class CrashTest : Command
    {
        public override void Execute()
        {
            Utils.ForceCrash(ForcedCrashCategory.Abort);
        }
    }
}