using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using Positiva_Task.Models;

namespace Positiva_Task.DataAccess
{
	public class UsersContext : DbContext
	{
		public UsersContext() : base("Positiva_DBEntities")
		{
		}

		public DbSet<User> Users { get; set; }

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
		}
	}
}