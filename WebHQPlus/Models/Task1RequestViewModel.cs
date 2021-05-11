using Microsoft.AspNetCore.Http;

namespace WebHQTest.Models
{
	public class Task1RequestViewModel
	{
		public IFormFile File { get; set; }
		public string Url { get; set; }
	}
}
