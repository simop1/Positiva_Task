using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Positiva_Task.Models
{
	public class LoginModel
	{
		public string UserName { get; set; }
		public string Password { get; set; }
	}
}