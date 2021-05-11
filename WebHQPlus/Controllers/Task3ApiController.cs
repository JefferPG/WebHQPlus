using Microsoft.AspNetCore.Mvc;
using System;
using WebHQTest.Models;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;
using System.Linq;

namespace WebHQTest.Controllers
{
	[Route("api/task3")]
	[ApiController]
	public class Task3ApiController : Controller
	{
		private static JsonSerializerOptions _serializeOptions;

		public Task3ApiController()
		{
			_serializeOptions = new JsonSerializerOptions
			{
				PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
			};
		}

		[HttpGet]
		public GeneralResponseViewModel Get([FromForm] FileRequestViewModel requestBody)
		{
			var response = new GeneralResponseViewModel();
			try
			{
				if (requestBody.File.Length > 0)
				{
					string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Files");

					if (!Directory.Exists(path))
					{
						Directory.CreateDirectory(path);
					}

					using FileStream stream = new FileStream(Path.Combine(path, requestBody.File.FileName), FileMode.Create);
					requestBody.File.CopyTo(stream);
				}

				response.Message = "Task 3 Api";
			}
			catch (Exception e)
			{
				response.Message = e.Message;
			}

			return response;
		}

		[HttpGet("{hotelID}/{arrivalDate}")]
		public Task3ResponseViewModel Get([FromForm] FileRequestViewModel request, string hotelID, string arrivalDate)
		{
			var response = new Task3ResponseViewModel();

			try
			{
				if (request.File == null || request.File.Length == 0)
				{
					response.Message = "JsonFile is required.";
					return response;
				}

				if (hotelID == null || hotelID.Trim().Equals("") || !int.TryParse(hotelID, out int numericValue))
				{
					response.Message = "HotelID is a numeric required.";
					return response;
				}

				if (arrivalDate == null || arrivalDate.Trim().Equals("") || !DateTime.TryParse(arrivalDate, out _))
				{
					response.Message = "ArrivalDate is date formet (yyyy-mm-dd) required.";
					return response;
				}

				string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Files");

				if (!Directory.Exists(path))
				{
					Directory.CreateDirectory(path);
				}

				using var reader = new StreamReader(request.File.OpenReadStream());
				var fileContent = reader.ReadToEnd();

				var requestJson = JsonSerializer.Deserialize<List<HotelRatesViewModel>>(fileContent, _serializeOptions);
				response.FilterList = requestJson.FindAll(h => h.Hotel.HotelID == int.Parse(hotelID));
				foreach (HotelRatesViewModel hotel in response.FilterList)
				{
					hotel.HotelRates = hotel.HotelRates.Where(s => DateTime.Parse(s.TargetDay).Date == DateTime.Parse(arrivalDate).Date);
				}

				response.Message = "Success!";
			}
			catch (Exception e)
			{
				response.Message = e.Message;
			}

			return response;
		}
	}
}
