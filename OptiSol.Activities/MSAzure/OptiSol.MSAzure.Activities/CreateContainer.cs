using System;
using System.Activities;
using System.ComponentModel;

namespace OptiSol.MSAzure {
    [DisplayName("Create Container")]
    [Description("Create the container in the Azure cloud storage")]
    public class CreateContainer : CodeActivity {
        [Category("Input")]
        [RequiredArgument]
        public InArgument<string> StorageConnectionString { get; set; }

        [Category("Input")]
        [RequiredArgument]
        public InArgument<string> StorageBlobContainer { get; set; }

        [Category("Output")]
        public OutArgument<bool> IsContainerCreated { get; set; }

        [Category("Common")]
        public ConstantString.EnumYesOrNo ContinueOnError { get; set; }

        /*
         * If ContinueOnError argument is 'true', return the exception else nope.
         */
        private void CrateContainerInAzureStorage(CodeActivityContext context) {
            try {
                AzureParamHelper azureStorageHelper = new AzureParamHelper(StorageConnectionString.Get(context), StorageBlobContainer.Get(context));
                IsContainerCreated.Set(context, AzureHelper.CrateContainerInAzureStorage());
            } catch (Exception ex) {
                if (Convert.ToBoolean((int)ContinueOnError)) {
                    } else {
                    throw ex;
                }
            }
        }

        protected override void Execute(CodeActivityContext context) {
            CrateContainerInAzureStorage(context);
        }
    }
}
