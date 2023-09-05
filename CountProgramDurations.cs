 namespace Terrasoft.Configuration.CountProgramDurationsNamespace
{
	using System;
	using System.ServiceModel;
	using System.ServiceModel.Web; 
	using System.ServiceModel.Activation;
	using System.Net.Http;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Net;
	using System.Data;
	using System.Linq;
	using Terrasoft.Core;
	using Terrasoft.Core.DB;
	using Terrasoft.Web.Common;
	using Terrasoft.Core.Entities;

	[ServiceContract]
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public class CountProgramDurations: BaseService
	{
		[OperationContract]
		[WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped,
		ResponseFormat = WebMessageFormat.Json)]
		public string GetDurations(string code) {
			try
			{
				var result = "";
				//Извлечение из БД Id искомой программы
				var select_program = new Select(UserConnection)
						.Column("Id")
					.From("UsrSwimmingPrograms")
					.Where("Code").IsEqual(Column.Parameter(code)) as Select;
				var program_id = select_program.ExecuteScalar<string>();
				//Извлечение из БД перечня продолжительностей занятий
				var select_classes = new Select(UserConnection)
						.Column("ClassDuration")
					.From("SwimmingClasses")
					.Where("UsrSwimmingProgramsId").IsEqual(Column.Parameter(program_id))
					.And("ClassStateId").IsEqual(Column.Parameter("4f91e6ca-61b9-4535-8f0f-d737fda65482")) as Select;
				using (DBExecutor dbExecutor = UserConnection.EnsureDBConnection())
				{
					using (IDataReader dataReader = select_classes.ExecuteReader(dbExecutor))
					{
						var list = new List<Object>();
						while (dataReader.Read())
						{
							var o = dataReader.GetValue(0);
							var t = "";
							if (o.ToString() == "")
							{
								t = "00:00:00";
							}
							else
							{
								t = o.ToString();
							}
							list.Add(t);
						}
						int total_minutes = 0;
						int total_hours = 0;
						foreach(string item in list)
						{
							var splitted = item.Split(':');
							var read_hours = int.Parse(splitted[0]);
							var read_minutes = int.Parse(splitted[1]);
							total_minutes += read_minutes;
							total_hours += read_hours;
						}
						if (total_minutes / 60 >= 1) {
							total_hours += total_minutes / 60;
							total_minutes %= 60;
						}
						if (total_hours > 0)
						{
							result += total_hours.ToString() + " часов";
						}
						if (total_minutes > 0)
						{	
							if (total_hours > 0)
							{
								result += ", ";
							}
							result += total_minutes.ToString() + " минут";
						}
						if (result == "")
						{
							return "Нет запланированных занятий, либо у них не указана продолжительность!";
						}
					}
				}
				return result;
			}
			catch (Exception e)
			{
				return "-1";
			}
		}
	}
}
