using System;
using System.Activities;
using System.ComponentModel;

namespace OptiSol.MSAzure {
    [DisplayName("Get Blob Names")]
    [Description("List out the blob names from the Azure cloud storage")]
    public class GetBlobNames : CodeActivity {
        [Category("Input")]
        [RequiredArgument]
        public InArgument<string> StorageConnectionString { get; set; }

        [Category("Input")]
        [RequiredArgument]
        public InArgument<string> StorageBlobContainer { get; set; }

        [Category("Input")]
        public ConstantString.EnumYesOrNo IsIncludedSubfolder { get; set; }

        [Category("Output")]
        public OutArgument<System.Collections.Generic.List<string>> BlobNameList { get; set; }

        [Category("Common")]
        public ConstantString.EnumYesOrNo ContinueOnError { get; set; }

        /*
         * If ContinueOnError argument is 'true', return the exception else nope.
         * useFlatBlobListing is true to ensure loading all files in virtual blob sub-folders as a plain list
         */
        private void GetBlobNamesFromAzureStorage(CodeActivityContext context) {
            try {
                AzureParamHelper azureStorageHelper = new AzureParamHelper(StorageConnectionString.Get(context), StorageBlobContainer.Get(context));
                BlobNameList.Set(context, AzureHelper.GetBlobNamesFromAzureStorage(Convert.ToBoolean((int)IsIncludedSubfolder)));
            } catch (Exception ex) {
                if (Convert.ToBoolean((int)ContinueOnError)) {
                } else {
                    throw ex;
                }
            }
        }

        protected override void Execute(CodeActivityContext context) {
            GetBlobNamesFromAzureStorage(context);
        }
    }
}
