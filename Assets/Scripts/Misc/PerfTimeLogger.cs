/*
    Re-Volt Track Editor - Unity Edition
    A version of the track editor re-built from the ground up in Unity
    Copyright (C) 2022 Dummiesman

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System.Diagnostics;

public class PerfTimeLogger
{
    private string logTag = null;
    private long lastElapsedMs = 0;
    private Stopwatch backingStopwatch;

    public void Log(string message)
    {
        long totalElapsedMs = backingStopwatch.ElapsedMilliseconds;
        long delta = totalElapsedMs - lastElapsedMs;

        if (logTag != null)
        {
            UnityEngine.Debug.Log($"[{logTag}] [d:{delta} e:{totalElapsedMs}] {message} ");
        }
        else
        {
            UnityEngine.Debug.Log($"[d:{delta} e:{totalElapsedMs}] {message} ");
        }

        lastElapsedMs = totalElapsedMs;
    }

    public void Pause()
    {
        backingStopwatch.Stop();
    }

    public void Resume()
    {
        backingStopwatch.Start();
    }

    public void Restart()
    {
        lastElapsedMs = 0;
        backingStopwatch.Restart();
    }

    public PerfTimeLogger()
    {
        backingStopwatch = Stopwatch.StartNew();
    }

    public PerfTimeLogger(string logTag) : this()
    {
        this.logTag = logTag;
    }
}
