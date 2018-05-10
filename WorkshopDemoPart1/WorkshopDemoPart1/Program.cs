using System;

namespace WorkshopDemoPart1
{
    class Program
    {
        static void Main(string[] args)
        {
            var blobStorageDemo = new BlobStorageDemo();

            blobStorageDemo.StoreAndLoadBlobInAzure()
                .GetAwaiter()
                .GetResult();


            WaitForKey();
        }

        internal static void WaitForKey()
        {
            Console.WriteLine("Waiting for key...");
            Console.ReadKey();
        }
    }
}
