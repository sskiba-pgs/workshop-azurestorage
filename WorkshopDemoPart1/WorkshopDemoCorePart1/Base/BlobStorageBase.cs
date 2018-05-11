using Microsoft.WindowsAzure.Storage.Auth;
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
        protected async Task OpenContainerAsync(string containerName, string blobName = "blobsdemo",  bool usingSasUrl = false)
        {
            // Create the CloudBlobClient that represents the Blob storage endpoint for the storage account.
            CloudBlobClient cloudBlobClient;
            if (usingSasUrl)
            {
                var sasToken = Config["StorageSASToken"];
                StorageCredentials credentials = new StorageCredentials(sasToken);
                cloudBlobClient = new CloudBlobClient(StorageAccount.BlobEndpoint, credentials);

            } else
            {
                cloudBlobClient = StorageAccount.CreateCloudBlobClient();
            }

            // Create a container called 'blobsDemo' and append a formated tday day indicator.
            CloudBlobContainer = cloudBlobClient.GetContainerReference(blobName);

            Console.WriteLine("Created new container '{0}'", CloudBlobContainer.Name);

            if (await CloudBlobContainer.CreateIfNotExistsAsync())
            {
                if (usingSasUrl == false)
                {
                    // Set the permissions so the blobs are public.
                    BlobContainerPermissions permissions = new BlobContainerPermissions
                    {
                        PublicAccess = BlobContainerPublicAccessType.Blob
                    };
                    await CloudBlobContainer.SetPermissionsAsync(permissions);

                    Console.WriteLine("Set blob permissions for container '{0}'", CloudBlobContainer.Name);
                }
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
