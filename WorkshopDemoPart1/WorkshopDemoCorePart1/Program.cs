using System;


namespace WorkshopDemoCorePart1
{
    class Program
    {
        static void Main(string[] args)
        {
            var blobStorageDemo = new BlobStorageDemo();

            //blobStorageDemo.StoreAndLoadBlobInAzure().GetAwaiter().GetResult();


            //blobStorageDemo.SendFileWithProgress().GetAwaiter().GetResult();

            //blobStorageDemo.SendEncryptedText().GetAwaiter().GetResult();


            blobStorageDemo.CreateSASPolicy("workshopPolicy").GetAwaiter().GetResult();

            var blobSasDemo = new BlobStorageSAS();

            blobSasDemo.TestSasAccessBlobs("sss").GetAwaiter().GetResult();

            WaitForKey();
        }

        internal static void WaitForKey()
        {
            Console.WriteLine("Waiting for key...");
            Console.ReadKey();
        }
    }
}
