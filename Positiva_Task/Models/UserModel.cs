using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Positiva_Task.Models
{
	public class UserModel
	{
		public int UserID { get; set; }
		[Required(ErrorMessage = "Required")]
		public string FirstName { get; set; }
		[Required(ErrorMessage = "Required")]
		public string LastName { get; set; }
		[Required(ErrorMessage = "Required")]
		public string UserName { get; set; }
		[Required(ErrorMessage = "Required")]
		public string Email { get; set; }
		[Required(ErrorMessage = "Required")]
		public string Password { get; set; }
		[Required(ErrorMessage = "Required")]
		public string DateOfBirth { get; set; }
	}
}