using System;
using System.Activities;
using System.ComponentModel;

namespace OptiSol.MSAzure {
    [DisplayName("Delete Blob Reference")]
    [Description("Delete the bolb reference from the Azure cloud storage")]
    public class DeleteFileFromBlob : CodeActivity {
        [Category("Input")]
        [RequiredArgument]
        public InArgument<string> StorageConnectionString { get; set; }

        [Category("Input")]
        [RequiredArgument]
        public InArgument<string> StorageBlobContainer { get; set; }

        [Category("Input")]
        [RequiredArgument]
        public InArgument<string> BlobReference { get; set; }

        [Category("Output")]
        public OutArgument<bool> IsDeleted { get; set; }

        [Category("Common")]
        public ConstantString.EnumYesOrNo ContinueOnError { get; set; }

        /*
         * If ContinueOnError argument is 'true', return the exception else nope.
         */
        private void DeleteBlobFromAzureStorage(CodeActivityContext context) {
            try {
                AzureParamHelper azureStorageHelper = new AzureParamHelper(StorageConnectionString.Get(context), StorageBlobContainer.Get(context));
                IsDeleted.Set(context, AzureHelper.DeleteBlobFromAzureStorage(BlobReference.Get(context)));
            } catch (Exception ex) {
                if (Convert.ToBoolean((int)ContinueOnError)) {
                    } else {
                    throw ex;
                }
            }
        }

        protected override void Execute(CodeActivityContext context) {
            DeleteBlobFromAzureStorage(context);
        }
    }
}
