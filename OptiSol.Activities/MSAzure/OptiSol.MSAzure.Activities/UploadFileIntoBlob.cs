using System;
using System.Activities;
using System.ComponentModel;

namespace OptiSol.MSAzure {
    [DisplayName("Upload File")]
    [Description("Upload a file into the Azure cloud storage")]
    public class UploadFileIntoBlob : CodeActivity {
        [Category("Input")]
        [RequiredArgument]
        public InArgument<string> StorageConnectionString { get; set; }

        [Category("Input")]
        [RequiredArgument]
        public InArgument<string> StorageBlobContainer { get; set; }

        [Category("Input")]
        [RequiredArgument]
        public InArgument<string> SourceFile { get; set; }

        [Category("Input")]
        [RequiredArgument]
        public InArgument<string> BlobReference { get; set; }

        [Category("Input")]
        public ConstantString.EnumYesOrNo DeleteSourceAfterUpload { get; set; }

        [Category("Output")]
        public OutArgument<string> BlobUri { get; set; }

        [Category("Common")]
        public ConstantString.EnumYesOrNo ContinueOnError { get; set; }

        /*
         * If ContinueOnError argument is 'true', return the exception else nope.
         */
        private void UploadToAzureStorage(CodeActivityContext context) {
            try {
                AzureParamHelper azureStorageHelper = new AzureParamHelper(StorageConnectionString.Get(context), StorageBlobContainer.Get(context));
                BlobUri.Set(context, AzureHelper.UploadToAzureStorage(SourceFile.Get(context), BlobReference.Get(context), Convert.ToBoolean((int)DeleteSourceAfterUpload)));
            } catch (Exception ex) {
                if (Convert.ToBoolean((int)ContinueOnError)) {
                } else {
                    throw ex;
                }
            }
        }

        protected override void Execute(CodeActivityContext context) {
            UploadToAzureStorage(context);
        }
    }
}
