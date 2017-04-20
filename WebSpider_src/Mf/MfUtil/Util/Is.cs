using System;

namespace Mf.Util
{
	/// <summary>
	/// Summary description for Is.
	/// </summary>
	public class Is
	{
      /// <summary> Class cannot be instantiated</summary>
      private Is()
		{
		}

      /// <summary> Empty String tests a String to see if it is null or empty.
      /// </summary>
      /// <param name="str">String to be tested.
      /// </param>
      /// <returns> boolean true if empty.
      /// </returns>
      public static bool EmptyString( String value )
      {
         return ( value == null || value.Trim().Length == 0 );
      }

      /// <summary> Empty HttpCookie tests a HttpCookie to see if it is null or empty.
      /// </summary>
      /// <param name="HttpCookie">HttpCookie to be tested.
      /// </param>
      /// <returns> boolean true if empty.
      /// </returns>
      public static bool EmptyHttpCookie( System.Web.HttpCookie value )
      {
         return ( value == null || value.Value == null || value.Value.Trim().Length == 0 );
      }

   }
}
