using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Core.Util;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;


namespace WorkshopDemoCorePart1
{
    internal class BlobStorageDemo: BlobStorageBase
    {
        public async Task StoreAndLoadBlobInAzure()
        {
            string sourceFile = null;
            string destinationFile = null;

            try
            {
                await OpenContainerAsync($"blobsdemo{DateTime.Today:yymmdd}");


                // Create a file in your local MyDocuments folder to upload to a blob.
                string localPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                string localFileName = "QuickStart_" + Guid.NewGuid().ToString() + ".txt";
                sourceFile = Path.Combine(localPath, localFileName);
                // Write text to the file.
                File.WriteAllText(sourceFile, "Hello, World!");

                Console.WriteLine("Temp file = {0}", sourceFile);
                Console.WriteLine("Uploading to Blob storage as blob '{0}'", localFileName);
                Console.WriteLine();

                // Get a reference to the blob address, then upload the file to the blob.
                // Use the value of localFileName for the blob name.
                CloudBlockBlob cloudBlockBlob = CloudBlobContainer.GetBlockBlobReference(localFileName);
                await cloudBlockBlob.UploadFromFileAsync(sourceFile);

                // List the blobs in the container.
                Console.WriteLine("Listing blobs in container.");
                BlobContinuationToken blobContinuationToken = null;
                do
                {
                    var results = await CloudBlobContainer.ListBlobsSegmentedAsync(null, blobContinuationToken);
                    // Get the value of the continuation token returned by the listing call.
                    blobContinuationToken = results.ContinuationToken;
                    foreach (IListBlobItem item in results.Results)
                    {
                        Console.WriteLine(item.Uri);
                    }
                } while (blobContinuationToken != null); // Loop while the continuation token is not null.
                Console.WriteLine();

                // Download the blob to a local file, using the reference created earlier.
                // Append the string "_DOWNLOADED" before the .txt extension so that you can see both files in MyDocuments.
                destinationFile = sourceFile.Replace(".txt", "_DOWNLOADED.txt");
                Console.WriteLine("Downloading blob to {0}", destinationFile);
                Console.WriteLine();
                await cloudBlockBlob.DownloadToFileAsync(destinationFile, FileMode.Create);
            }
            catch (StorageException ex)
            {
                Console.WriteLine("Error returned from the service: {0}", ex.Message);
            }
            finally
            {
                Console.WriteLine("Press Y to delete the sample files and example container.");
                var key = Console.ReadKey();
                Console.WriteLine();

                if (key.KeyChar.ToString().ToUpper() == "Y")
                {
                    await DeleteContainerAsync();

                    Console.WriteLine("Deleting the local source file and local downloaded files");
                    Console.WriteLine();
                    File.Delete(sourceFile);
                    File.Delete(destinationFile);
                }
            }
        }

        public async Task SendFileWithProgress()
        {
            CancellationToken cancellationToken = new CancellationToken();
            IProgress<StorageProgress> progressHandler = new Progress<StorageProgress>(
                progress => Console.WriteLine("Progress: {0} bytes transferred", progress.BytesTransferred)
                );
            try
            {
                await OpenContainerAsync($"blobsdemo{DateTime.Today:yymmdd}");

                CloudBlockBlob cloudBlockBlob = CloudBlobContainer.GetBlockBlobReference("bigfile.img");

                using (Stream srcStream = new FileStream("c:\\workshop\\bigfile.img", FileMode.Open))
                {
                    await cloudBlockBlob.UploadFromStreamAsync(
                    srcStream,
                    default(AccessCondition),
                    default(BlobRequestOptions),
                    default(OperationContext),
                    progressHandler,
                    cancellationToken
                    );
                }
            }
            finally
            {
                Console.WriteLine("Press Y to delete Azure container.");
                var key = Console.ReadKey();
                Console.WriteLine();

                if (key.KeyChar.ToString().ToUpper() == "Y")
                {
                    await DeleteContainerAsync();
                }
            }

        }

        public async Task CheckEncryption()
        {
            try
            {
                await OpenContainerAsync($"blobsdemo{DateTime.Today:yymmdd}");

                CloudBlockBlob blob = CloudBlobContainer.GetBlockBlobReference("encrypted.txt");


                await blob.UploadTextAsync("test");

                await blob.FetchAttributesAsync();
                Console.WriteLine($"Blob is encrypted: {blob.Properties.IsServerEncrypted}");

                CloudBlockBlob testBlob = CloudBlobContainer.GetBlockBlobReference(blob.Name);
                await testBlob.DownloadTextAsync();
                Console.WriteLine($"Blob is encrypted: {testBlob.Properties.IsServerEncrypted}");
            }
            finally
            {
                Console.WriteLine("Press Y to delete Azure container.");
                var key = Console.ReadKey();
                Console.WriteLine();

                if (key.KeyChar.ToString().ToUpper() == "Y")
                {
                    await DeleteContainerAsync();
                }
            }
        }

        public async Task CreateSASPolicy(string policyName)
        {
            await OpenContainerAsync($"blobsdemo{DateTime.Today:yymmdd}");


            BlobContainerPermissions permissions = await CloudBlobContainer.GetPermissionsAsync();

            // Create a policy with read access.
            SharedAccessBlobPolicy policy = new SharedAccessBlobPolicy()
            {
                SharedAccessStartTime = DateTimeOffset.UtcNow.AddMinutes(-5),
                SharedAccessExpiryTime = DateTimeOffset.UtcNow.AddMinutes(5),
                Permissions = SharedAccessBlobPermissions.Read
            };

            CloudBlockBlob blob = CloudBlobContainer.GetBlockBlobReference("SAS.txt");
            await blob.UploadTextAsync("test");

            string sasBlobToken = blob.GetSharedAccessSignature(policy);

            Console.WriteLine($"SasToken: {sasBlobToken}");

            permissions.SharedAccessPolicies.Add(policyName, policy);

            await CloudBlobContainer.SetPermissionsAsync(permissions);
        }

    }
}
