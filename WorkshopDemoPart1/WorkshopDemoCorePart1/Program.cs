using System;


namespace WorkshopDemoCorePart1
{
    class Program
    {
        static void Main(string[] args)
        {
            var blobStorageDemo = new BlobStorageDemo();

            blobStorageDemo.StoreAndLoadBlobInAzure().GetAwaiter().GetResult();


            //blobStorageDemo.SendFileWithProgress().GetAwaiter().GetResult();

            //blobStorageDemo.CheckEncryption().GetAwaiter().GetResult();


            //blobStorageDemo.CreateSASPolicy("workshopPolicy").GetAwaiter().GetResult();

            //blobStorageDemo.OpenBlobUsingSAS().GetAwaiter().GetResult();

            //blobStorageDemo.CreateSASUriForBlob().GetAwaiter().GetResult();            
        }     
    }
}
