using System;
using System.Activities;
using System.ComponentModel;

namespace OptiSol.MSAzure {
    [DisplayName("Get Used Space")]
    [Description("Get the used space in bytes of the Azure cloud storage")]
    public class GetUsedSpace : CodeActivity {
        [Category("Input")]
        [RequiredArgument]
        public InArgument<string> StorageConnectionString { get; set; }

        [Category("Input")]
        [RequiredArgument]
        public InArgument<string> StorageBlobContainer { get; set; }

        [Category("Input")]
        public InArgument<string> FileExtension { get; set; }

        [Category("Output")]
        public OutArgument<long> UsedSpace { get; set; }

        [Category("Common")]
        public ConstantString.EnumYesOrNo ContinueOnError { get; set; }

        /*
         * If ContinueOnError argument is 'true', return the exception else nope.
         * Used storage space size will return in bytes
         */
        private void GetUsedSpaceOfAzureStorage(CodeActivityContext context) {
            try {
                AzureParamHelper azureStorageHelper = new AzureParamHelper(StorageConnectionString.Get(context), StorageBlobContainer.Get(context));
                UsedSpace.Set(context, AzureHelper.GetUsedSpaceOfAzureStorage(FileExtension.Get(context)));
            } catch (Exception ex) {
                if (Convert.ToBoolean((int)ContinueOnError)) {
                    } else {
                    throw ex;
                }
            }
        }

        protected override void Execute(CodeActivityContext context) {
            GetUsedSpaceOfAzureStorage(context);
        }
    }
}
