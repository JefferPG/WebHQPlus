using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Collections.Generic;
using WebHQTest.Models;
using SpreadsheetLight;
using System.Data;

namespace WebHQTest.Controllers
{
	[Route("api/task2")]
	[ApiController]
	public class Task2ApiController : Controller
	{
		[HttpGet]
		public ActionResult<IEnumerable<string>> Get()
		{
			return new string[] { "Task 2" };
		}

		[HttpPost]
		public GeneralResponseViewModel Post([FromBody] HotelRatesViewModel request)
		{
			GeneralResponseViewModel response = new GeneralResponseViewModel();
			try
			{
				string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Files");

				if (!Directory.Exists(path))
				{
					Directory.CreateDirectory(path);
				}

				string file = Path.Combine(path,DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HHmmss") + ".xlsx");
				
				SLDocument document = new SLDocument();

				DataTable dt = new DataTable();

				dt.Columns.Add("ARRIVAL_DATE", typeof(string));
				dt.Columns.Add("DEPARTURE_DATE", typeof(string));
				dt.Columns.Add("PRICE", typeof(double));
				dt.Columns.Add("CURRENCY", typeof(string));
				dt.Columns.Add("RATENAME", typeof(string));
				dt.Columns.Add("ADULTS", typeof(int));
				dt.Columns.Add("BREAKFAST_INCLUDED", typeof(int));

				foreach (var item in request.HotelRates)
				{
					string arrivalDate = DateTime.Parse(item.TargetDay).ToString("dd'.'MM'.'yyyy");
					string departureDate = DateTime.Parse(item.TargetDay).AddDays(1).ToString("dd'.'MM'.'yyyy");
					decimal price = item.Price.NumericFloat;
					string currency = item.Price.Currency;
					string rateName = item.RateName;
					int adults = item.Adults;
					int breakFast = 0;
					foreach (var tag in item.RateTags)
					{
						if (tag.Name.Equals("breakfast"))
						{
							breakFast = Convert.ToInt32(tag.Shape);
						}
					}

					dt.Rows.Add(arrivalDate, departureDate, price, currency, rateName, adults, breakFast);
				}

				document.ImportDataTable(1, 1, dt, true);

				document.SaveAs(file);

				response.Message = "Success, excel created: " + file;
			}
			catch (Exception e) {
				response.Message = e.Message.ToString();
			}
			
			return response;
		}
	}
}
