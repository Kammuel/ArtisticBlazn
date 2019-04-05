namespace BlaznWings
{
	using BlaznWings.Models;
	using System;
	using System.Data.Entity;
	using System.Linq;

	public class AdminDB : DbContext
	{
		// Your context has been configured to use a 'AdminDB' connection string from your application's 
		// configuration file (App.config or Web.config). By default, this connection string targets the 
		// 'BlaznWings.AdminDB' database on your LocalDb instance. 
		// 
		// If you wish to target a different database and/or database provider, modify the 'AdminDB' 
		// connection string in the application configuration file.
		public AdminDB()
			: base("name=AdminDB")
		{
		}

		// Add a DbSet for each entity type that you want to include in your model. For more information 
		// on configuring and using a Code First model, see http://go.microsoft.com/fwlink/?LinkId=390109.

		public virtual DbSet<AdminUser> Admins { get; set; }
	}

	//public class MyEntity
	//{
	//    public int Id { get; set; }
	//    public string Name { get; set; }
	//}
}