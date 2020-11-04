namespace CuotasApp.Models
{
	public class User
	{
		public string Username { get; set; }
		public string Password { get; set; }
		public double Usage { get; set; }
		public double Today { get; set; }
		public double Last_week { get; set; }
		public double Max { get; set; }

		public string Year { get; set; }
		public string Online_time_24 { get; set; }
		public string Online_time_week { get; set; }
		public string Online_time_month { get; set; }
		public string Online_time_year { get; set; }

		public User()
		{
		}
	}
}
