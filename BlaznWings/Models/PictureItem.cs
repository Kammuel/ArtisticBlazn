using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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

		[NotMapped]
		public HttpPostedFileBase Path { get; set; }

	}
}