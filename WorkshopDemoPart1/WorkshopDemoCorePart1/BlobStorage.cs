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
        public async Task OpenBlobUsingSAS()
        {
            try
            {
                await OpenContainerAsync($"blobsdemoSAS{DateTime.Today:yymmdd}", "sas-blob", true);

                CloudBlockBlob blob = CloudBlobContainer.GetBlockBlobReference("CreatedBySas.txt");
                
                await blob.UploadTextAsync("File created using SAS Uri");                

                CloudBlockBlob testBlob = CloudBlobContainer.GetBlockBlobReference(blob.Name);

                var fileContent = await testBlob.DownloadTextAsync();

                Console.WriteLine("File text: {0}",fileContent);                
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
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

        public async Task CreateSASUriForBlob()
        {
            try
            {
                await OpenContainerAsync($"blobsdemoSAS{DateTime.Today:yymmdd}");


                Console.WriteLine("Creating file in storage container");

                CloudBlockBlob blob = CloudBlobContainer.GetBlockBlobReference("FileWithSASUri.txt");

                await blob.UploadTextAsync("File with SAS Uri generated from .NET");
                                
                CloudBlockBlob testBlob = CloudBlobContainer.GetBlockBlobReference(blob.Name);

                var sasUri = GetBlobSasUri(testBlob);

                Console.WriteLine("Blob SAS Uri: {0}", sasUri);
                Console.WriteLine("Hit any key...");

                Console.WriteLine();

                Console.ReadKey();

                Console.WriteLine("Opening blob using SAS Uri");

                CloudBlockBlob blobThruSas = new CloudBlockBlob(new Uri(sasUri));

                Console.WriteLine("File text: {0}", await testBlob.DownloadTextAsync());


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


        private string GetBlobSasUri(CloudBlob blob, string policyName = null)
        {
            string sasBlobToken;
            
            if (policyName == null)
            {
                // Create a new access policy and define its constraints.
                // Note that the SharedAccessBlobPolicy class is used both to define the parameters of an ad-hoc SAS, and
                // to construct a shared access policy that is saved to the container's shared access policies.
                SharedAccessBlobPolicy adHocSAS = new SharedAccessBlobPolicy()
                {
                    // When the start time for the SAS is omitted, the start time is assumed to be the time when the storage service receives the request.
                    // Omitting the start time for a SAS that is effective immediately helps to avoid clock skew.
                    SharedAccessExpiryTime = DateTime.UtcNow.AddHours(24),
                    SharedAccessStartTime = DateTime.UtcNow.AddHours(-1),
                    Permissions = SharedAccessBlobPermissions.Read | SharedAccessBlobPermissions.Write | SharedAccessBlobPermissions.Create
                };

                // Generate the shared access signature on the blob, setting the constraints directly on the signature.
                sasBlobToken = blob.GetSharedAccessSignature(adHocSAS);

                Console.WriteLine("SAS for blob (ad hoc): {0}", sasBlobToken);
                Console.WriteLine();
            }
            else
            {
                // Generate the shared access signature on the blob. In this case, all of the constraints for the
                // shared access signature are specified on the container's stored access policy.
                sasBlobToken = blob.GetSharedAccessSignature(null, policyName);

                Console.WriteLine("SAS for blob (stored access policy): {0}", sasBlobToken);
                Console.WriteLine();
            }

            // Return the URI string for the container, including the SAS token.
            return blob.Uri + sasBlobToken;
        }

    }
}
