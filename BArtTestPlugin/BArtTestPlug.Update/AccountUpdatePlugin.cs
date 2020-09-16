using System;
using Microsoft.Xrm.Sdk;

namespace BArtTestPlug.Update
{
    public class AccountUpdatePlugin : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            // Obtain the execution context from the service provider.
            IPluginExecutionContext context =
                (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            // Checked action.
            if (context.MessageName != "Update")
                return;

            if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
            {

                Entity account = (Entity)context.InputParameters["Target"];

                // Checked entity.
                if (account.LogicalName != "account")
                    return;

                // Get a reference to the Organization service.
                IOrganizationServiceFactory factory =
                    (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));

                IOrganizationService service = factory.CreateOrganizationService(context.UserId);

                try
                {
                    // Plug-in logic.
                    Entity newAccount = new Entity("account") { Id = account.Id };

                    newAccount.Attributes["new_isvalidated"] = account.Attributes.ContainsKey("telephone1")
                                                               && !string.IsNullOrEmpty(account.Attributes["telephone1"]
                                                                   .ToString());

                    if (context.Depth <= 1)
                    {
                        service.Update(newAccount);
                    }
                }
                catch (Exception e)
                {
                    throw new InvalidPluginExecutionException(e.Message);
                }

            }
        }
    }
}
