using UnityEngine;
using System.IO;
using System.Timers;

namespace CPULog
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    class CPULog : MonoBehaviour
    {
        private int updatedFrameCount = 0;
        private float time = 0;

        Timer cTimer;
        ConfigNode node = new ConfigNode();
        string path = Path.Combine(new DirectoryInfo(KSPUtil.ApplicationRootPath).FullName, "GameData/CPULog.txt").Replace("\\", "/");

        private void Start()
        {
            cTimer = new System.Timers.Timer(1000);
            cTimer.Elapsed += new ElapsedEventHandler(onTimerEvent);
            cTimer.Enabled = true;
        }

        private void onTimerEvent(object source, ElapsedEventArgs e)
        {            
            int frame = Time.frameCount;
            time += 1; 
            int fps = frame - updatedFrameCount;
            updatedFrameCount = frame;
            saveToFile(time, fps);
            if (node.CountValues > 2000)
            {
                node.ClearValues();
                time = 0;
                updatedFrameCount = 0;
            }
        }
        
        private void saveToFile(float time, int entry)
        {
            node.AddValue(time.ToString(), entry.ToString());
            if (node.Save(path)) print("values saved");

        }

        private void OnDestroy()
        {
            cTimer.Stop();
        }
    }
}
