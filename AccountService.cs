namespace Terrasoft.Configuration.AccountService
{
  using System;
  using System.ServiceModel;
  using System.ServiceModel.Web;
  using System.ServiceModel.Activation;
  using System.Threading.Tasks;
  using System.Net.Http;
  using System.Collections;
  using System.Net;
  using Terrasoft.Core;
  using Terrasoft.Core.DB;
  using Terrasoft.Web.Common;
  using Terrasoft.Core.Entities;

    [ServiceContract]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class AccountService: BaseService
    {
		  private SystemUserConnection _systemUserConnection;
      private SystemUserConnection SystemUserConnection {
        get {
          return _systemUserConnection ?? (_systemUserConnection = (SystemUserConnection)AppConnection.SystemUserConnection);
        }
      }
		
      [OperationContract]
      [WebInvoke(Method = "GET", RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped,
      ResponseFormat = WebMessageFormat.Json)]
      public string AccountRequest(string sub) {
        SessionHelper.SpecifyWebOperationIdentity(HttpContextAccessor.GetInstance(), SystemUserConnection.CurrentUser);
  			try
  			{
  				var select_accounts = new Select(SystemUserConnection)
  						.Column(Func.Count("Name")).As("AccountCount")
  					.From("Account")
  					.Where("Name").ConsistsWith(Column.Parameter(sub)) as Select;
  				var cnt = select_accounts.ExecuteScalar<string>();
  				return cnt;
  			}
  			catch (Exception e)
  			{
  				return e.ToString();
  			}
      }
	  }
}
