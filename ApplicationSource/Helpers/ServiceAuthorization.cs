using System;
using System.Collections.Generic;
using System.Web;

namespace ApplicationSource.Helpers
{
	public static class ServiceAuthorization
	{
		public static void Authorize()
		{
			var serviceAuthUser = HttpContext.Current.Session["User"] as User;

			if(string.IsNullOrEmpty(serviceAuthUser?.Name))
				ThrowSessionTimeoutException();
		}

		public static void ThrowSessionTimeoutException()
		{
			throw new TimeoutException();
		}
	}
}