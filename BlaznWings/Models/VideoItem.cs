using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BlaznWings.Models
{
	public class VideoItem
	{
		[Key]
		public int VideoItemID { get; set; }

		[Required]
		public string Name { get; set; }

		public string Description { get; set; }

		public string Category { get; set; }

		public string Url { get; set; }

		public static IEnumerable<SelectListItem> GetCategories()
		{
			yield return new SelectListItem { Text = "Anime", Value = "Anime" };
			yield return new SelectListItem { Text = "Tutorial", Value = "Tutorial" };
		}
	}
}