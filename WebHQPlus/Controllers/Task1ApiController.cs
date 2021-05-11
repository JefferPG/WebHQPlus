using HtmlAgilityPack;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ScrapySharp.Extensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using WebHQTest.Models;
using static WebHQTest.Models.Task1ResponseViewModel;

namespace WebHQTest.Controllers
{
	[Route("api/task1")]
	[ApiController]
	public class Task1ApiController : Controller
	{
		[HttpGet]
		public ActionResult<IEnumerable<string>> Get()
		{
			return new string[] { "Task 1" };
		}

		[HttpPost]
		public Task1ResponseViewModel Post([FromForm] Task1RequestViewModel request)
		{
			if (request == null || (request.File == null && request.Url == null))
			{
				return new Task1ResponseViewModel
				{
					Message = "File (HtmlFile) or url is required."
				};
			}

			if (request.Url != null && !request.Url.Trim().Equals(""))
			{
				return ScrapUrl(request.Url);
			}

			if (request.File != null && request.File.Length > 0)
			{
				return ScrapFile(request.File);
			}

			return null;
		}

		private string RemoveBreakLine(string val)
		{
			return val.Replace("\n", "").Replace("\t", "").Trim();
		}

		private Task1ResponseViewModel ScrapUrl(string url)
		{
			Task1ResponseViewModel response = new Task1ResponseViewModel();
			try
			{
				HtmlWeb htmlWeb = new HtmlWeb();
				HtmlDocument htmlDoc = htmlWeb.Load(url);

				HtmlNode htmlTitle = htmlDoc.DocumentNode.CssSelect("div[class='hp__hotel-title']").First();

				// Obtaining name;
				foreach (var node in htmlDoc.DocumentNode.CssSelect("h2[id='hp_hotel_name']"))
				{
					node.RemoveChild(node.SelectSingleNode("span"));
					response.Name = RemoveBreakLine(node.InnerText);
				}

				// Obtaining Clasification/Stars
				foreach (var node in htmlDoc.DocumentNode.CssSelect("div[class='hp__hotel-title']"))
				{
					foreach (var child in node.CssSelect("span[class='bui-icon bui-rating__item bui-icon--medium']"))
					{
						response.Clasification += 1;
					}
				}

				// Obtaining Address
				foreach (var node in htmlDoc.DocumentNode.CssSelect("span[data-node_tt_id='location_score_tooltip']"))
				{
					response.Address = RemoveBreakLine(node.InnerText);
				}

				// Obtaining Review
				foreach (var node in htmlDoc.DocumentNode.CssSelect("div[id='js--hp-gallery-scorecard']"))
				{
					foreach (var child in node.CssSelect("div[class='bui-review-score__badge']"))
					{
						response.Review = decimal.Parse(RemoveBreakLine(child.FirstChild.InnerText), CultureInfo.InvariantCulture);
					}
				}

				// Obtaining Description
				foreach (var node in htmlDoc.DocumentNode.CssSelect("div[id='summary']"))
				{
					foreach (var child in node.CssSelect("p"))
					{
						response.Description += child.InnerText;
					}
				}
				response.Description = RemoveBreakLine(response.Description + "");

				// Obtaining Rooms Category
				foreach (var node in htmlDoc.DocumentNode.CssSelect("table[id='maxotel_rooms']"))
				{
					foreach (var child in node.CssSelect("a[class='jqrt togglelink']"))
					{
						response.RoomCategory.Add(RemoveBreakLine(child.InnerText));
					}
				}

				// Obtaining Alternative Hotels

				response.Url = url.Substring(0, url.IndexOf("?") > 0 ? url.IndexOf("?") : url.Length);
				response.Message = "Success!";
			}
			catch (Exception e)
			{
				response.Message = e.Message.ToString();
			}

			return response;
		}

		private Task1ResponseViewModel ScrapFile(IFormFile file)
		{
			Task1ResponseViewModel response = new Task1ResponseViewModel();
			try
			{
				HtmlDocument htmlDoc = new HtmlDocument();

				using var reader = new StreamReader(file.OpenReadStream());

				htmlDoc.LoadHtml(reader.ReadToEnd());

				// Obtaining name;
				foreach (var node in htmlDoc.DocumentNode.CssSelect("span[id='hp_hotel_name']"))
				{
					response.Name = RemoveBreakLine(node.InnerText);
				}

				// Obtaining Clasification/Stars
				foreach (var node in htmlDoc.DocumentNode.CssSelect("span[class='bui-icon bui-rating__item bui-icon--medium']"))
				{
					response.Clasification += 1;
				}

				// Obtaining Address
				foreach (var node in htmlDoc.DocumentNode.CssSelect("span[id='hp_address_subtitle']"))
				{
					response.Address = RemoveBreakLine(node.InnerText);
				}

				// Obtaining Review
				foreach (var node in htmlDoc.DocumentNode.CssSelect("div[id='js--hp-gallery-scorecard']"))
				{
					foreach (var child in node.CssSelect("span[class='average js--hp-scorecard-scoreval']"))
					{
						response.Review = decimal.Parse(RemoveBreakLine(child.FirstChild.InnerText), CultureInfo.InvariantCulture);
					}
				}

				// Obtaining Description
				foreach (var node in htmlDoc.DocumentNode.CssSelect("div[class='hotel_description_wrapper_exp ']"))
				{
					foreach (var child in node.CssSelect("p"))
					{
						response.Description += child.InnerText;
					}
				}
				response.Description = RemoveBreakLine(response.Description + "");

				// Obtaining Rooms Category
				foreach (var node in htmlDoc.DocumentNode.CssSelect("table[id='maxotel_rooms']"))
				{
					foreach (var child in node.CssSelect("td[class='ftd']"))
					{
						response.RoomCategory.Add(RemoveBreakLine(child.InnerText));
					}
				}

				// Obtaining Alternative Hotels
				foreach (var node in htmlDoc.DocumentNode.CssSelect("table[id='althotelsTable']"))
				{
					foreach (var child in node.CssSelect("td[class='althotelsCell tracked']"))
					{
						var alternative = new AlternativeHotel();
						foreach (var name in child.CssSelect("a[class='althotel_link']"))
						{
							alternative.Url = name.GetAttributeValue("href");
							alternative.Url = alternative.Url.Substring(0, alternative.Url.IndexOf("?") > 0 ? alternative.Url.IndexOf("?") : alternative.Url.Length);
							alternative.Name = RemoveBreakLine(name.InnerText);
						}
						foreach (var description in child.CssSelect("span[class='hp_compset_description']"))
						{
							alternative.Description = RemoveBreakLine(description.InnerText);
						}
						foreach (var review in child.CssSelect("span[class='average js--hp-scorecard-scoreval']"))
						{
							alternative.Review = decimal.Parse(RemoveBreakLine(review.InnerText), CultureInfo.InvariantCulture);
						}
						response.Alternative.Add(alternative);
					}
				}

				// Obtaining Alternative Url
				foreach (var node in htmlDoc.DocumentNode.CssSelect("input[class='share_center_url']"))
				{
					response.Url = node.GetAttributeValue("content");
				}

				response.Message = "Success!";
			}
			catch (Exception e)
			{
				response.Message = e.Message.ToString();
			}

			return response;
		}
	}
}
