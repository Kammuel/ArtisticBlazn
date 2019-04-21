using BlaznWings.Models;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
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
		public ActionResult AddPhotosAsync()
		{

			return View();
		}

		[HttpPost]
		public async System.Threading.Tasks.Task<ActionResult> AddPhotosAsync(PictureItem picture)
		{
			AdminDB db = new AdminDB();

			PictureItem nameCheck = db.Pictures.Where(v => v.Name.ToLower().
			Equals(picture.Name.ToLower())).SingleOrDefault();

			if (nameCheck == null)
			{

				if (picture.Path != null)
				{
					CloudStorageAccount storageAccount = new CloudStorageAccount(
					new Microsoft.WindowsAzure.Storage.Auth.StorageCredentials("blaznart", "9Mh4A2zUSK8QgDOCKwf/OL7uqAtFDG90uoPNpJYXAlteZVqJU6xkN9/IP1zWVoV0l2WfFce4ksZ7t4fFHLXeaA=="), true);

					CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

					CloudBlobContainer container = blobClient.GetContainerReference("blaznphotos");

					// Get a reference to a blob named "myblob".
					CloudBlockBlob blockBlob = container.GetBlockBlobReference("myblob");


					string imageName = picture.Name;

					blockBlob = container.GetBlockBlobReference(imageName);
					blockBlob.Properties.ContentType = picture.Path.ContentType;

					await blockBlob.UploadFromStreamAsync(picture.Path.InputStream);

					CloudBlockBlob addedBlob = GetPictureBlobByName(imageName);

					if (addedBlob != null)
					{
						PictureItem newPicture = new PictureItem();
						db = new AdminDB();

						newPicture.Category = picture.Category;
						newPicture.Description = picture.Description;
						//newPicture.Path = picture.Path;
						newPicture.Name = picture.Name;
						newPicture.Url = addedBlob.Uri.ToString();

						db.Pictures.Add(newPicture);
						db.SaveChanges();

						ViewBag.Message = "Picture added to storage!";
					}
					else
					{
						ViewBag.Message = "Unable to add that blob for whatever reason.";

						return View();
					}

					ViewBag.Message = "Image uploaded!";

					return RedirectToAction("AddPhotosAsync");
				}
				else
				{
					ViewBag.Message = "Please select a file.";

					return RedirectToAction("AddPhotosAsync");
				}
			}
			else
			{
				ViewBag.ErrorMessage = "A picture with that name already exists.";

				return View();
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

		[HttpGet]
		public ActionResult EditDeletePhotos()
		{
			AdminDB db = new AdminDB();

			List<PictureItem> pictures = db.Pictures.ToList();

			return View(pictures);
		}

		[HttpGet]
		public ActionResult EditPhotoAsync(int id)
		{
			AdminDB db = new AdminDB();

			PictureItem thisPhoto = db.Pictures.Where(v => v.PictureItemID == id).SingleOrDefault();

			if(thisPhoto != null)
			{
				return View(thisPhoto);
			}
			else
			{
				ViewBag.Message = "Could not find that picture's ID.";

				return RedirectToAction("EditDeletePhotos");
			}
		}

		[HttpPost]
		public async System.Threading.Tasks.Task<ActionResult> EditPhotoAsync(PictureItem picture)
		{
			//Set up database
			AdminDB db = new AdminDB();

			PictureItem oldPicture = db.Pictures.
				Where(v => v.PictureItemID == picture.PictureItemID).SingleOrDefault();

			//Set up Azure blob connection
			CloudStorageAccount storageAccount = new CloudStorageAccount(
				new Microsoft.WindowsAzure.Storage.
				Auth.StorageCredentials("blaznart", "9Mh4A2zUSK8QgDOCKwf/OL7uqAtFDG90uoPNpJYXAlteZVqJU6xkN9/IP1zWVoV0l2WfFce4ksZ7t4fFHLXeaA=="), true);

			CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

			CloudBlobContainer container = blobClient.GetContainerReference("blaznphotos");

			//Delete Old Blob in Storage
			//Delete blob
			CloudBlockBlob blob = container.GetBlockBlobReference(oldPicture.Name);

			CloudBlockBlob blobCopy = container.GetBlockBlobReference(picture.Name);

			if (!await blobCopy.ExistsAsync())
			{

				if (await blob.ExistsAsync())
				{
					await blobCopy.StartCopyAsync(blob);
					await blob.DeleteIfExistsAsync();
				}
			}

			//Change PictureItem in SQL database
			picture.Url = blobCopy.Uri.ToString();

			db.Pictures.AddOrUpdate(picture);
			db.SaveChanges();

			return RedirectToAction("EditDeletePhotos");
		}

		[HttpGet]
		public ActionResult DeletePhoto (int id)
		{
			AdminDB db = new AdminDB();

			PictureItem deletePicture = db.Pictures.Where(v => v.PictureItemID == id).SingleOrDefault();

			if(deletePicture != null)
			{
				//Delete blob
				CloudStorageAccount storageAccount = new CloudStorageAccount(
				new Microsoft.WindowsAzure.Storage.Auth.StorageCredentials("blaznart", "9Mh4A2zUSK8QgDOCKwf/OL7uqAtFDG90uoPNpJYXAlteZVqJU6xkN9/IP1zWVoV0l2WfFce4ksZ7t4fFHLXeaA=="), true);

				CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

				CloudBlobContainer container = blobClient.GetContainerReference("blaznphotos");

				CloudBlockBlob blob = container.GetBlockBlobReference(deletePicture.Name);

				blob.Delete();

				//Delete Picture from database
				db.Pictures.Remove(deletePicture);

				db.SaveChanges();

				return RedirectToAction("EditDeletePhotos");
			}
			else
			{
				return RedirectToAction("EditDeletePhotos");
			}
		}

		public CloudBlockBlob GetPictureBlobByName(string Name)
		{
			CloudStorageAccount storageAccount = new CloudStorageAccount(
				new Microsoft.WindowsAzure.Storage.Auth.StorageCredentials("blaznart", "9Mh4A2zUSK8QgDOCKwf/OL7uqAtFDG90uoPNpJYXAlteZVqJU6xkN9/IP1zWVoV0l2WfFce4ksZ7t4fFHLXeaA=="), true);

			CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

			CloudBlobContainer container = blobClient.GetContainerReference("blaznphotos");

			CloudBlockBlob potentialItem;

			potentialItem = container.GetBlockBlobReference(Name);

			return potentialItem;
		}

	}
}