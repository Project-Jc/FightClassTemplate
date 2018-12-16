
namespace GcdManage
{
    public static class GcdManager
    {
        private static int Delay => 1600;

        private static int Tick { get; set; }

        public static bool GcdIsOff => System.Environment.TickCount > Tick;

        public static void Update() => Tick = (System.Environment.TickCount + Delay);

        public static void Init() => Tick = System.Environment.TickCount;
    }
}


