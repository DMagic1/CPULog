using UnityEngine;
using System.IO;
using System.Timers;

namespace CPULog
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    class CPULog : MonoBehaviour
    {
        private int updatedFrameCount = 0;
        private int time = 0;

        Timer cTimer;
        ConfigNode node = new ConfigNode();
        string path = Path.Combine(new DirectoryInfo(KSPUtil.ApplicationRootPath).FullName, "GameData/CPULog.txt").Replace("\\", "/");

        private void Start()
        {
            if (FlightGlobals.ActiveVessel.name == "Mark1-2Pod (CPU Test 600 v23_5)") GameEvents.onStageActivate.Add(TimerStart);
        }

        private void TimerStart(int stage)
        {
            if (stage == 14)
            {
                print("Starting Timer");
                GameEvents.onStageActivate.Remove(TimerStart);
                node.ClearValues();
                node.AddValue("Begin Run", System.DateTime.Now.ToString());
                cTimer = new System.Timers.Timer(1000);
                cTimer.Elapsed += new ElapsedEventHandler(onTimerEvent);
                cTimer.Enabled = true;
            }
            else GameEvents.onStageActivate.Remove(TimerStart);
        }

        private void onTimerEvent(object source, ElapsedEventArgs e)
        {
            int frame = Time.frameCount;
            time += 1; 
            int fps = frame - updatedFrameCount;
            updatedFrameCount = frame;
            saveToFile(time, fps);
        }
        
        private void saveToFile(int time, int entry)
        {
            node.AddValue(time.ToString(), entry.ToString());
            node.Save(path);
        }

        private void OnDestroy()
        {
            print("Destroying Timer");
            cTimer.Stop();
            GameEvents.onStageActivate.Remove(TimerStart);
        }
    }
}
