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
		public GeneralResponseViewModel Get([FromForm] UploadRequest requestBody)
		{
			var response = new GeneralResponseViewModel();
			try
			{
				if (requestBody.JsonFile.Length > 0)
				{
					string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Files");

					if (!Directory.Exists(path))
					{
						Directory.CreateDirectory(path);
					}

					using FileStream stream = new FileStream(Path.Combine(path, requestBody.JsonFile.FileName), FileMode.Create);
					requestBody.JsonFile.CopyTo(stream);
				}

				response.Message = "Task 3 Api";
			}
			catch (Exception e)
			{
				response.Message = e.Message;
			}

			return response;
		}

		[HttpGet("{HotelID}/{ArrivalDate}")]
		public Task3ResponseViewModel Get([FromForm] UploadRequest request, string HotelID, string ArrivalDate)
		{
			var response = new Task3ResponseViewModel();

			try
			{
				if (request.JsonFile == null || request.JsonFile.Length == 0)
				{
					response.Message = "JsonFile is required.";
					return response;
				}

				if (HotelID == null || HotelID.Trim().Equals("") || !int.TryParse(HotelID, out int numericValue))
				{
					response.Message = "HotelID is a numeric required.";
					return response;
				}

				if (ArrivalDate == null || ArrivalDate.Trim().Equals("") || !DateTime.TryParse(ArrivalDate, out _))
				{
					response.Message = "ArrivalDate is date formet (yyyy-mm-dd) required.";
					return response;
				}

				string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Files");

				if (!Directory.Exists(path))
				{
					Directory.CreateDirectory(path);
				}

				using var reader = new StreamReader(request.JsonFile.OpenReadStream());
				var fileContent = reader.ReadToEnd();

				var requestJson = JsonSerializer.Deserialize<List<HotelRatesViewModel>>(fileContent, _serializeOptions);
				response.FilterList = requestJson.FindAll(h => h.Hotel.HotelID == int.Parse(HotelID));
				foreach (HotelRatesViewModel hotel in response.FilterList)
				{
					hotel.HotelRates = hotel.HotelRates.Where(s => DateTime.Parse(s.TargetDay).Date == DateTime.Parse(ArrivalDate).Date);
				}

				response.Message = "Success!";
			}
			catch (Exception e)
			{
				response.Message = e.Message;
			}

			return response;
		}

		public class UploadRequest
		{
			public IFormFile JsonFile { get; set; }
		}
	}
}
