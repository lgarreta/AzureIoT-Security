// Obtain an access token and uses it to create a resource group in Azure
using System;
using Microsoft.Azure.Management.ResourceManager;
using Microsoft.Azure.Management.ResourceManager.Models;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Rest;

namespace AzureIoTProgTest {
	class AzureIoTProgClass {
		// Replace de corresponding values taken from the autentication process
		static string tmpApplicationId = "4c1f4d33-e87b-4395-b758-dbc0d4afe25f";
		static string tmpSubscriptionId = "933a5bc9-18cb-4cd3-800a-2a2fdd692fb6";
		static string tmpTenantId = "2707f6c8-6127-4a28-858a-1bad498d78b8";
		static string tmpAppPassword = "iothub2017A";

		// Temporal names for resources
		static string tmpResourceGroupName = "progrg1";
		static string tmpResourceGroupLocation = "East US";

		static void Main(string[] args) {
			// Retrieve a token from Azure AD using the application id and password
			var accessToken = GetAuthorizationToken (tmpApplicationId, tmpSubscriptionId, tmpTenantId, tmpAppPassword);

			// Create, or obtain a reference to, the resource group you are using
			var rgResponse = CreateUpdateResourceGroup (accessToken, tmpSubscriptionId, tmpResourceGroupName, tmpResourceGroupLocation);
			Console.WriteLine("End, press some key");
			Console.ReadLine();
		}

		//----------------------------------------------------------------------
		// Return the resource management client by retrieve a token from Azure AD
		//----------------------------------------------------------------------
		private static string	GetAuthorizationToken (string applicationId, string subscriptionId, string tenantId, string appPassword)	{
			Console.WriteLine("Getting authorization token credentials" + "...");
			var credential = new ClientCredential(applicationId, appPassword);
			var authContext = new AuthenticationContext(string.Format("https://login.windows.net/{0}", tenantId));
			AuthenticationResult token = authContext.AcquireTokenAsync("https://management.core.windows.net/", credential).Result;
			if (token == null) {
				throw new InvalidOperationException("Failed to obtain the token");
			}
			return token.AccessToken;
		}

		//----------------------------------------------------------------------
		// Create, or obtain a reference to, the resource group you are using
		//----------------------------------------------------------------------
		private static ResourceGroup CreateUpdateResourceGroup(string accessToken, string subscriptionId, string rgName, string rgLocalization) {
			Console.WriteLine("Creating/Updating resource group: " + rgName + "...");

			var tokenCredentials = new TokenCredentials(accessToken);
			var rmClient = new ResourceManagementClient(tokenCredentials) {SubscriptionId = subscriptionId};

			var rgResponse = rmClient.ResourceGroups.CreateOrUpdate(rgName, new ResourceGroup(rgLocalization));
			if (rgResponse.Properties.ProvisioningState != "Succeeded") {
				throw new InvalidOperationException("Failed to creating/updating resource group");
			}
			Console.WriteLine("Resource Group:\n" + rgResponse.ToString());
			return rgResponse;
		}
	}
}
