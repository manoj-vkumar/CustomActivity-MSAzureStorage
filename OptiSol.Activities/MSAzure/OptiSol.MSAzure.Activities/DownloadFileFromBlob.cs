using System;
using System.Activities;
using System.ComponentModel;

namespace OptiSol.MSAzure {
    [DisplayName("Download File")]
    [Description("Download a file from the Azure cloud storage")]
    public class DownloadFileFromBlob : CodeActivity {
        [Category("Input")]
        [RequiredArgument]
        public InArgument<string> StorageConnectionString { get; set; }

        [Category("Input")]
        [RequiredArgument]
        public InArgument<string> StorageBlobContainer { get; set; }

        [Category("Input")]
        [RequiredArgument]
        public InArgument<string> BlobReference { get; set; }

        [Category("Input")]
        [RequiredArgument]
        public InArgument<string> DestinationFilePath { get; set; }

        [Category("Output")]
        public OutArgument<bool> IsDownloaded { get; set; }

        [Category("Common")]
        public ConstantString.EnumYesOrNo ContinueOnError { get; set; }

        /*
         * If ContinueOnError argument is 'true', return the exception else nope.
         */
        private void DownloadFromAzureStorage(CodeActivityContext context) {
            try {
                AzureParamHelper azureStorageHelper = new AzureParamHelper(StorageConnectionString.Get(context), StorageBlobContainer.Get(context));
                IsDownloaded.Set(context, AzureHelper.DownloadFromAzureStorage(BlobReference.Get(context), DestinationFilePath.Get(context)));
            } catch (Exception ex) {
                if (Convert.ToBoolean((int)ContinueOnError)) {
                } else {
                    throw ex;
                }
            }
        }

        protected override void Execute(CodeActivityContext context) {
            DownloadFromAzureStorage(context);
        }
    }
}
