using System;
using System.Activities;
using System.ComponentModel;

namespace OptiSol.MSAzure {
    [DisplayName("Blob Exist")]
    [Description("Check the blob reference is exist or not in the Azure cloud storage")]
    public class CheckBlobExist : CodeActivity {
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
        public OutArgument<bool> IsBlobExist { get; set; }

        [Category("Common")]
        public ConstantString.EnumYesOrNo ContinueOnError { get; set; }

        /*
         * If ContinueOnError argument is 'true', return the exception else nope.
         */
        private void IsBlobExistsInAzureStorage(CodeActivityContext context) {
            try {
                AzureParamHelper azureStorageHelper = new AzureParamHelper(StorageConnectionString.Get(context), StorageBlobContainer.Get(context));
                IsBlobExist.Set(context, AzureHelper.IsBlobExistsInAzureStorage(BlobReference.Get(context)));
            } catch (Exception ex) {
                if (Convert.ToBoolean((int)ContinueOnError)) {
                    } else {
                    throw ex;
                }
            }
        }

        protected override void Execute(CodeActivityContext context) {
            IsBlobExistsInAzureStorage(context);
        }
    }
}
