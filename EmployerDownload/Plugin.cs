using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PhoneApp.Domain;
using PhoneApp.Domain.Attributes;
using PhoneApp.Domain.DTO;
using PhoneApp.Domain.Interfaces;

namespace EmployerDownloadPlugin
{
    [Author(Name = "Mihail Pavlov")]
    public class Plugin : IPluggable
    {
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
   
        public IEnumerable<DataTransferObject> Run(IEnumerable<DataTransferObject> args)
        {
            logger.Info("Donwload Employees from JSON");
            return JsonConvertToEmployeersJson(JsonReader("https://dummyjson.com/users")).ConvertTOEmployerDTO().Cast<DataTransferObject>();
        }
        private static string JsonReader(string url) => new WebClient().DownloadString(url);
        private static EmployeesJson JsonConvertToEmployeersJson (string json) => JsonConvert.DeserializeObject<EmployeesJson>(json);

        private class EmployeesJson
        {
            [JsonProperty("users")]
            public EmployeeJSON[] employees;
            public List<EmployeesDTO> ConvertTOEmployerDTO()
            {
                List<EmployeesDTO> employees = new List<EmployeesDTO>();
                foreach (var user in this.employees)
                {
                    EmployeesDTO employee = new EmployeesDTO();
                    employee.Name = string.Format($"{user.firstname} {user.lastname}");
                    employee.AddPhone(user.phone);
                    employees.Add(employee);
                }
                return employees;
            }

        }
        private class EmployeeJSON
        {
            public string firstname { get; set; }
            public string lastname { get; set; }
            public string phone { get; set; }

        }
    }
}
