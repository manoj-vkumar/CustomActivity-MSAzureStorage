using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.IO;
using System.Linq;

namespace OptiSol.MSAzure {
    public class AzureParamHelper {
        public static string _storageBlobContainer_cdn { get; set; }
        public static string _storageConnectionString { get; set; }

        public AzureParamHelper(string StorageBlobContainer_cdn, string StorageConnectionString) {
            _storageBlobContainer_cdn = StorageBlobContainer_cdn;
            _storageConnectionString = StorageConnectionString;
        }
    }

    public class AzureHelper {
        private static CloudStorageAccount storageAccount;
        private static CloudBlobClient blobClient;
        private static readonly CloudBlobContainer blobContainer;

        /** Initialize the CloudStorageAccount, CloudBlobClient, CloudBlobContainer **/
        static AzureHelper() {
            storageAccount = CloudStorageAccount.Parse(AzureParamHelper._storageBlobContainer_cdn);
            blobClient = storageAccount.CreateCloudBlobClient();
            blobContainer = blobClient.GetContainerReference(AzureParamHelper._storageConnectionString);
        }

        /* 
         * Take the source file name as the destination file.
         * If the given blob reference is already exist it'll return a empty string, else proceed to upload the file.
         * Convert a source file into a file stream and then upload it into the Azure storage blob.
         * If deleteSourceAfterUpload parameter value is a 'true', delete the source file.
         */
        public static string UploadToAzureStorage(string sourceFilePath, string destinationFilePath, bool deleteSourceAfterUpload) {
            string filename = Path.GetFileName(sourceFilePath);
            string blobReference = (destinationFilePath + "/" + filename).Trim();
            CloudBlockBlob blockBlob = blobContainer.GetBlockBlobReference(blobReference);
            if (blockBlob.Exists()) {
                return string.Empty;
            }

            using (FileStream fileStream = File.OpenRead(sourceFilePath)) {
                blockBlob.UploadFromStream(fileStream);
            }

            if (deleteSourceAfterUpload) {
                File.Delete(sourceFilePath);
            }
            return blockBlob.Uri.AbsoluteUri;
        }

        /*
         * If the given blob reference is already exist it'll return false, else proceed.
         * Download a file as a file stream and then write it.
         */
        public static bool DownloadFromAzureStorage(string blobReference, string destinationPath) {
            CloudBlockBlob blockBlob = blobContainer.GetBlockBlobReference(blobReference);
            if (!blockBlob.Exists()) {
                return false;
            }

            using (FileStream filestream = File.OpenWrite(destinationPath)) {
                blockBlob.DownloadToStream(filestream);
            }

            blockBlob.DownloadToFile(destinationPath, FileMode.OpenOrCreate);
            return true;
        }

        /*
         * If the given blob reference is already exist it'll return false, else proceed.
         * Delete the blob is exist.
         */
        public static bool DeleteBlobFromAzureStorage(string blobReference) {
            CloudBlockBlob blockBlob = blobContainer.GetBlockBlobReference(blobReference);
            if (!blockBlob.Exists()) {
                return false;
            }

            blockBlob.DeleteIfExists();
            return true;
        }

        /*
         * If IsIncludedSubfolder parrameter is 'true', list all the blob names includes sub-folders.
         */
        public static System.Collections.Generic.List<string> GetBlobNamesFromAzureStorage(bool IsIncludedSubfolder) {
            return blobContainer.ListBlobs(useFlatBlobListing: IsIncludedSubfolder).OfType<CloudBlockBlob>().Select(b => b.Name).ToList();
        }

        /*
         * Create a container only if exist.
         */
        public static bool CrateContainerInAzureStorage() {
            blobContainer.CreateIfNotExists();
            return true;
        }

        /*
         * If fileExtension parrameter is 'true', return that specific extension files size.
         * Used storage space size will return in bytes
         */
        public static long GetUsedSpaceOfAzureStorage(string fileExtension) {
            if (blobContainer.Exists()) {
                return (from CloudBlockBlob blob in
                            blobContainer.ListBlobs(useFlatBlobListing: true).Where(x => string.IsNullOrEmpty(fileExtension) || x.Uri.AbsolutePath.Contains(fileExtension.ToLower().Trim()))    //useFlatBlobListing - Consider the sub-folders
                        select blob.Properties.Length
                  ).Sum();
            }
            return 0;
        }

        /*
         * Return 'true', if the given blob rference is exist.
         */
        public static bool IsBlobExistsInAzureStorage(string blobReference) {
            CloudBlockBlob blockBlob = blobContainer.GetBlockBlobReference(blobReference);
            return blockBlob.Exists();
        }
    }
}
