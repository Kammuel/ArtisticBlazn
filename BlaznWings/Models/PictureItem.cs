using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BlaznWings.Models
{
	public class PictureItem
	{
		[Key]
		public int PictureItemID { get; set; }

		[Required]
		public string Name { get; set; }

		[Required]
		public string Category { get; set; }

		public string Description { get; set; }

		public string Url { get; set; }

		public static IEnumerable<SelectListItem> GetCategories()
		{
			yield return new SelectListItem { Text = "Portrait", Value = "Portrait" };
			yield return new SelectListItem { Text = "Commission", Value = "Commission" };
		}
	}
}