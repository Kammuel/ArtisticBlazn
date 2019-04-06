using BlaznWings.Models;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace BlaznWings.Controllers
{
	public class HomeController : Controller
	{
		[HttpGet]
		public ActionResult Index()
		{
			return View();
		}

		[HttpGet]
		public ActionResult AdminPortal()
		{
			return View();
		}

		[HttpPost]
		public ActionResult AdminPortal(AdminUser user)
		{
			AdminDB db = new AdminDB();

			AdminUser checkUser = db.Admins.Where(v => v.Username.Equals(user.Username)).SingleOrDefault();

			if(checkUser == null)
			{
				ViewBag.UserMessage = "That username does not exist.";

				return View();
			}
			else
			{
				if(user.Password.Equals(checkUser.Password))
				{


					return RedirectToAction("AdminLanding", "Home");
				}
				else
				{
					ViewBag.PasswordMessage = "That password is incorrect";

					return View();
				}
			}
		}

		[HttpGet]
		public ActionResult AdminLanding()
		{
			return View();
		}

		[HttpGet]
		public ActionResult ManagePhotos()
		{
			return View();
		}
		
		[HttpGet]
		public ActionResult AddPhotos()
		{

			return View();
		}

		[HttpPost]
		public async System.Threading.Tasks.Task<ActionResult> AddPhotosAsync(HttpPostedFileBase file)
		{
			if (file != null)
			{
				CloudStorageAccount storageAccount = new CloudStorageAccount(
				new Microsoft.WindowsAzure.Storage.Auth.StorageCredentials("blaznart", "9Mh4A2zUSK8QgDOCKwf/OL7uqAtFDG90uoPNpJYXAlteZVqJU6xkN9/IP1zWVoV0l2WfFce4ksZ7t4fFHLXeaA=="), true);

				CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

				CloudBlobContainer container = blobClient.GetContainerReference("blaznphotos");

				// Get a reference to a blob named "myblob".
				CloudBlockBlob blockBlob = container.GetBlockBlobReference("myblob");
				

				string imageName = Guid.NewGuid().ToString() + "-" + Path.GetExtension(file.FileName);

				blockBlob = container.GetBlockBlobReference(imageName);
				blockBlob.Properties.ContentType = file.ContentType;

				await blockBlob.UploadFromStreamAsync(file.InputStream);


				ViewBag.Message = "Image uploaded!";

				return RedirectToAction("AddPhotos");
			}
			else
			{
				ViewBag.Message = "Please select a file.";

				return RedirectToAction("AddPhotos");
			}
		}

		[HttpGet]
		public async System.Threading.Tasks.Task<ActionResult> EditDeletePhotosAsync()
		{
			CloudStorageAccount storageAccount = new CloudStorageAccount(
				new Microsoft.WindowsAzure.Storage.Auth.StorageCredentials("blaznart", "9Mh4A2zUSK8QgDOCKwf/OL7uqAtFDG90uoPNpJYXAlteZVqJU6xkN9/IP1zWVoV0l2WfFce4ksZ7t4fFHLXeaA=="), true);

			CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

			CloudBlobContainer container = blobClient.GetContainerReference("blaznphotos");

			List<string> images = new List<string>();

			BlobContinuationToken token = null;
			do
			{
				BlobResultSegment resultSegment = await container.ListBlobsSegmentedAsync(token);
				token = resultSegment.ContinuationToken;

				foreach (IListBlobItem item in resultSegment.Results)
				{
					images.Add(item.Uri.ToString());
				}
			}
			while (token != null);

			ViewBag.Images = images;
			ViewBag.Current = "ManagePicturesAsync";

			return View();
		}

	}
}