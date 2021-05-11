using System.Collections.Generic;

namespace WebHQTest.Models
{
	public class Task3ResponseViewModel : GeneralResponseViewModel
	{
		public Task3ResponseViewModel()
		{
			FilterList = new List<HotelRatesViewModel>();
		}

		public List<HotelRatesViewModel> FilterList { get; set; }
	}
}
