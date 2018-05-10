using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WorkshopDemoCorePart1
{
    internal class BlobStorageBase: BaseAzureDemo
    {
        public CloudBlobContainer CloudBlobContainer { get; set; }
        protected async Task OpenContainerAsync(string containerName)
        {
            // Create the CloudBlobClient that represents the Blob storage endpoint for the storage account.
            CloudBlobClient cloudBlobClient = StorageAccount.CreateCloudBlobClient();

            // Create a container called 'blobsDemo' and append a formated tday day indicator.
            CloudBlobContainer = cloudBlobClient.GetContainerReference($"blobsdemo");

            if (await CloudBlobContainer.CreateIfNotExistsAsync())
            {
                // Set the permissions so the blobs are public.
                BlobContainerPermissions permissions = new BlobContainerPermissions
                {
                    PublicAccess = BlobContainerPublicAccessType.Blob
                };
                await CloudBlobContainer.SetPermissionsAsync(permissions);

                Console.WriteLine("Created new container '{0}'", CloudBlobContainer.Name);
            }
            else
            {
                Console.WriteLine("Connected to existing container '{0}'", CloudBlobContainer.Name);
            }
            Console.WriteLine();
        }

        protected async Task DeleteContainerAsync()
        {
            // Clean up resources. This includes the container and the two temp files.
            Console.WriteLine("Deleting the container and any blobs it contains");
            if (CloudBlobContainer != null)
            {
                await CloudBlobContainer.DeleteIfExistsAsync();
            }
        }
    }
}
