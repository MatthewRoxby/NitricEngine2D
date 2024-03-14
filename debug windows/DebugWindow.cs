namespace NitricEngine2D.debug_windows
{
    /// <summary>
    /// Abstract class that defines a debug window
    /// </summary>
    public abstract class DebugWindow
    {
        /// <summary>
        /// update the debug window by a delta time
        /// </summary>
        /// <param name="deltaTime">time in seconds since last update</param>
        public virtual void Update(float deltaTime)
        {

        }

        /// <summary>
        /// dispose of the window once scene changes or application quits
        /// </summary>
        public virtual void Dispose()
        {

        }
    }
}
