using System.Runtime.InteropServices;

namespace StreamingAssetsInjector.Runtime
{
    public static class StreamingAssetsLoader
    {
        [DllImport("__Internal")]
        public static extern string LoadStreamingAssetsData();
    }
}