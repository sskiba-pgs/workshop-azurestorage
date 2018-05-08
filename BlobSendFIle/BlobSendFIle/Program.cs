using System;

namespace BlobSendFIle
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create the CloudBlobClient that represents the Blob storage endpoint for the storage account.
            CloudBlobClient cloudBlobClient = storageAccount.CreateCloudBlobClient();

            // Create a container called 'workshopblobs' and append a GUID value to it to make the name unique.
            var cloudBlobContainer = cloudBlobClient.GetContainerReference("workshopblobs" + Guid.NewGuid().ToString());
            await cloudBlobContainer.CreateAsync();

            // Set the permissions so the blobs are public.
            BlobContainerPermissions permissions = new BlobContainerPermissions
            {
                PublicAccess = BlobContainerPublicAccessType.Blob
            };
            await cloudBlobContainer.SetPermissionsAsync(permissions);
        }
    }
}
