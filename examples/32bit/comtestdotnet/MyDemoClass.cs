using System;
using System.Runtime.InteropServices;

namespace comtestdotnet
{
    [Guid("8982943B-B0B2-4619-BBDC-A9DBBD71B97C")]
    [ComVisible(true)]
    [ComDefaultInterface(typeof(IMyDemoInterface))]
    public class MyDemoClass : IMyDemoInterface
    {
        public void HelloWorld()
        {
            if (Environment.Is64BitProcess)
            {
                Console.WriteLine($"Hello from .NET! I am a 64 bit process");
            }
            else
            {
                Console.WriteLine($"Hello from .NET! I am a 32 bit process");
            }
        }
    }

    [Guid("7D2FFD8B-4BD6-4AE4-9CA0-A253EF9C48E1")]
    [ComVisible(true)]
    public interface IMyDemoInterface
    {
        void HelloWorld();
    }
}