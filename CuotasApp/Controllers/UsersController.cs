using System;
using System.IO;
using System.Linq;
using System.Net;
using CuotasApp.Models;
using HtmlAgilityPack;
using LDAP_Utils;
using Microsoft.AspNetCore.Mvc;

namespace CuotasApp.Controllers
{
	[Route("api/[controller]")]
	public class UsersController : Controller
	{
		[HttpGet]
		public IActionResult Index()
		{
			return Ok();
		}

		[HttpPost]
		public IActionResult Post([FromBody] User user)
		{
			LDAPController controller = new LDAPController(Environment.GetEnvironmentVariable("LDAP_HOST"));
			var ldap_user = controller.GetUserData(user.Username);

			if (ldap_user.DN == null)
				return NotFound(new {Error = String.Format("Usuario deshabilitado o inexistente.")});

			if (controller.CheckUser(ldap_user.DN, user.Password))
			{
				var data = GetData();

				for (int i = 0; i < data.GetLength(0); i++)
				{
					if (data[i, 0] == user.Username)
					{
						var str1 = data[i, 3];
						var str2 = data[i, 2];
						var str3 = data[i, 1];

						Console.WriteLine(str1);
						Console.WriteLine(str2);
						Console.WriteLine(str3);

						user.Usage = ConvertToMb(str1);
						user.Last_week = ConvertToMb(str2);
						user.Today = ConvertToMb(str3);

						user.Year = data[i, 4];
						user.Online_time_24 = data[i, 5];
						user.Online_time_week = data[i, 6];
						user.Online_time_month = data[i, 7];
						user.Online_time_year = data[i, 8];

						user.Max = GetMax(user.Username);
						return Ok(user);
					}
				}

				user.Usage = 0;
				user.Max = GetMax(user.Username);
				return Ok(user);
			}

			return NotFound(new {Error = String.Format("Contraseña incorrecta.")});
		}

		private double ConvertToMb(string line)
		{
			double result = 0;
			if (line.Length > 1)
			{
				var letter = line.Substring(line.Length - 2).ToLower();
				var number = line.Substring(0, line.Length - 2);

				if (letter == "gb") result = double.Parse(number) * 1024;
				else if (letter == "mb") result = double.Parse(number);
				else if (letter == "kb") result = Math.Round(double.Parse(number) / 1024, 2);
			}

			return result;
		}


		private string[,] GetData()
		{
			var url = Environment.GetEnvironmentVariable("SQUISH_URL");
			var web = new HtmlWeb();
			var doc = web.Load(url);


			var node = doc.DocumentNode.Descendants("");

			var element = doc.DocumentNode
				.Element("html")
				.Element("body");

			var trs = element.Element("table").Elements("tr").ToArray();
			var array = new string[trs.Length, 9];

			for (int row = 0; row < trs.Length; row++)
			{
				var tds = trs[row].Elements("td").ToArray();

				for (int col = 0; col < tds.Length; col++)
				{
					array[row, col] = tds[col].InnerText;
				}
			}

			return array;
		}

		private double GetMax(string UserName)
		{
			HttpWebRequest request = (HttpWebRequest) WebRequest.Create(Environment.GetEnvironmentVariable("SQUISH_URL")+"servicio");

			var response = request.GetResponse();
			var stream = response.GetResponseStream();

			StreamReader reader = new StreamReader(stream);
			string reg = "";
			while (!reader.EndOfStream)
			{
				string line = reader.ReadLine();
				if (line.StartsWith(UserName))
				{
					reg = line;
					break;
				}
			}

			if (reg != "")
			{
				string[] arr = reg.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries);
				var max = arr[1].Split("/")[0];
				return ConvertToMb(max);
			}

			return 500;
		}
	}
}
