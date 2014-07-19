/* CPULog
 * Benchmarking program designed for use with CPU test rocket
 * 
 * See here for more details: http://forum.kerbalspaceprogram.com/threads/42877-CPU-Performance-Database
 *
 * Copyright (c) 2014, David Grandy <david.grandy@gmail.com>
 * All rights reserved.
 * 
 * Redistribution and use in source and binary forms, with or without modification, 
 * are permitted provided that the following conditions are met:
 * 
 * 1. Redistributions of source code must retain the above copyright notice, 
 * this list of conditions and the following disclaimer.
 * 
 * 2. Redistributions in binary form must reproduce the above copyright notice, 
 * this list of conditions and the following disclaimer in the documentation and/or other materials 
 * provided with the distribution.
 * 
 * 3. Neither the name of the copyright holder nor the names of its contributors may be used 
 * to endorse or promote products derived from this software without specific prior written permission.
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, 
 * INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE 
 * DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, 
 * SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
 * GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF 
 * LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT 
 * OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 *  
 */

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

        private Timer cTimer;
        private ConfigNode node = new ConfigNode();
        private string path = Path.Combine(new DirectoryInfo(KSPUtil.ApplicationRootPath).FullName, "GameData/CPUDatabase/CPULog.txt").Replace("\\", "/");

        private void Start()
        {
            if (FlightGlobals.ActiveVessel.vesselName == "CPU Test 600 v23_5") {
                print("[CPULogger] CPU test rocket found");
                GameEvents.onStageActivate.Add(TimerStart);
            }
            else print("[CPULogger] No CPU test rocket found; shutting down");
        }

        private void TimerStart(int stage)
        {
            if (stage == 14)
            {
                print("[CPULogger] Starting timer");
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
            print("[CPULogger] Shutting down");
            cTimer.Stop();
            GameEvents.onStageActivate.Remove(TimerStart);
        }
    }
}
