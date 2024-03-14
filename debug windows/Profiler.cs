using ImGuiNET;

namespace NitricEngine2D.debug_windows
{
    /// <summary>
    /// debug window that provides information on the performance of the application
    /// </summary>
    public class Profiler : DebugWindow
    {
        int FPS;
        float peakFPS;
        float troughFPS = 1000;

        float totalFPS;

        float peakDelta;
        float troughDelta = 100f;
        float totalDelta;

        int totalFrames = -3;

        const int GRAPH_ITEMS = 100;

        //graph data is stored as a queue so that old data can be efficiently discarded
        Queue<float> deltaGraph = new Queue<float>();
        Queue<float> FPSgraph = new Queue<float>();

        //should the profiler show information in terms of FPS or milliseconds?
        bool showFPS = true;

        public override void Update(float deltaTime)
        {
            totalFrames++;
            //allows for a certain amount of setup frames as these will have really long times
            if(totalFrames < 0) { return; }


            FPS = (int)(1f / deltaTime);
            if(FPS > peakFPS)
            {
                peakFPS = FPS;
            }

            if(FPS < troughFPS)
            {
                troughFPS = FPS;
            }

            totalFPS += FPS;

            if(deltaTime > peakDelta)
            {
                peakDelta = deltaTime;
            }

            if(deltaTime < troughDelta)
            {
                troughDelta = deltaTime;
            }

            totalDelta += deltaTime;

            FPSgraph.Enqueue(FPS);
            if (FPSgraph.Count > GRAPH_ITEMS) { FPSgraph.Dequeue(); }

            deltaGraph.Enqueue(deltaTime * 1000f);
            if(deltaGraph.Count > GRAPH_ITEMS) { deltaGraph.Dequeue(); }

            if (ImGui.Begin("Profiler"))
            {
                if (ImGui.Button("Toggle FPS/delta")) { showFPS = !showFPS; }

                if (showFPS)
                {
                    ImGui.Text("Peak FPS: " + peakFPS.ToString());
                    ImGui.Text("Trough FPS: " + troughFPS.ToString());
                    ImGui.Text("Current FPS: " + FPS.ToString());
                    ImGui.Text("Average FPS: " + (totalFPS / totalFrames).ToString());
                    float[] a = FPSgraph.ToArray();
                    ImGui.PlotLines("graph", ref a[0], a.Length, 0, "", 0f, 100f);
                }
                else
                {
                    ImGui.Text("Peak ms: " + (peakDelta * 1000f).ToString());
                    ImGui.Text("Trough ms: " + (troughDelta * 1000f).ToString());
                    ImGui.Text("Current ms: " + (deltaTime * 1000f).ToString());
                    ImGui.Text("Average ms: " + (totalDelta / totalFrames * 1000f).ToString());
                    float[] a = deltaGraph.ToArray();
                    ImGui.PlotLines("graph", ref a[0], a.Length, 0, "", 0f, 50f);
                }
            }
        }

    }
}
