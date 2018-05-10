using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WorkshopDemoCorePart1
{
    internal class BlobStorageSAS: BlobStorageBase
    {
        const string containerSAS = "sasaccesscontainer";
        const string blobSAS = "blob.txt";
        const string containerSASWithAccessPolicy = "containerwithsaspolicy";
        const string blobSASWithAccessPolicy = "blobwithsaspolicy";

        public async Task TestSasAccessBlobs(string sas)
        {
            await OpenContainerAsync(containerSAS);

            // Create a list to store blob URIs returned by a listing operation on the container.
            List<IListBlobItem> blobList = new List<IListBlobItem>();

            //Write operation: write a new blob to the container.
            try
            {
                CloudBlockBlob blob = CloudBlobContainer.GetBlockBlobReference("blobCreatedViaSAS.txt");
                string blobContent = "This blob was created with a shared access signature granting write permissions to the container. ";
                await blob.UploadTextAsync(blobContent);

                Console.WriteLine("Write operation succeeded for SAS " + sas);
                Console.WriteLine();
            }
            catch (StorageException e)
            {
                Console.WriteLine("Write operation failed for SAS " + sas);
                Console.WriteLine("Additional error information: " + e.Message);
                Console.WriteLine();
            }


            //List operation: List the blobs in the container.
            try
            {
                OperationContext context = new OperationContext();
                BlobRequestOptions options = new BlobRequestOptions();

                BlobResultSegment segment = await CloudBlobContainer.ListBlobsSegmentedAsync(null, true, BlobListingDetails.All, null, null, options, context);
                foreach (IListBlobItem listblobItem in segment.Results)
                {
                    blobList.Add(listblobItem);
                }
                Console.WriteLine("List operation succeeded for SAS " + sas);
                Console.WriteLine();
            }
            catch (StorageException e)
            {
                Console.WriteLine("List operation failed for SAS " + sas);
                Console.WriteLine("Additional error information: " + e.Message);
                Console.WriteLine();
            }
        }


    }
}
