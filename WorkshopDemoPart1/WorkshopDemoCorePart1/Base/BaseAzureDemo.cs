using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using System;
using System.IO;

namespace WorkshopDemoCorePart1
{
    internal class BaseAzureDemo
    {
        protected string StorageConnectionString { get; set; }
        protected CloudStorageAccount StorageAccount => _storageAccount;

        public BaseAzureDemo()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            var configuration = builder.Build();

            StorageConnectionString = configuration["StorageConnectionString"];

            if (String.IsNullOrWhiteSpace(StorageConnectionString))
            {
                StorageConnectionString = Environment.GetEnvironmentVariable("storageconnectionstring");
            }
            if (String.IsNullOrWhiteSpace(StorageConnectionString)) {
                Console.WriteLine("StorageConnectionString is not defined!!!");
                Console.ReadKey();
                throw new Exception("Incorrect StorageConnectionString");
            }

            if (!CloudStorageAccount.TryParse(StorageConnectionString, out _storageAccount))
            {
                Console.WriteLine(
                    "A connection string has not been defined in the system environment variables or app.config. " +
                    "Add a environment variable named 'storageconnectionstring' with your storage " +
                    "connection string as a value.");
                Console.ReadKey();
                throw new Exception("Error in parsing StorageConnectionString");

            }
        }

        private CloudStorageAccount _storageAccount;
    }
}
