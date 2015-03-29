using System;
using System.Collections;

namespace Rock.StringFormatting
{
    public static class ToOctetStringExtension
    {
        /// <summary>
        /// Converts an encoded GUID to an octet string.
        /// </summary>
        /// <param name="value">A GUID.</param>
        /// <returns>An octet string representation of the GUID.</returns>
        /// <remarks>
        /// Converts the guid id in the format of xxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx to \xx\xx\xx\xx\xx\xx\xx\xx\xx\xx\xx
        /// </remarks>
        public static string ToOctetString(this Guid value)
        {
            string strGuid = value.ToString("N");
            string slash = "\\";
            // TODO:  find a better way to do convert the GUID to an octect string, maybe use a stringbuilder?			
            ArrayList list = new ArrayList();

            for (int i = 0; i <= 15; i++)
            {
                list.Add(strGuid.Substring(i * 2, 2).ToString());
            }
            // the string we want to return
            string strOctet;

            strOctet = slash + list[3].ToString() + slash +
                list[2].ToString() + slash +
                list[1].ToString() + slash +
                list[0].ToString() + slash +
                list[5].ToString() + slash +
                list[4].ToString() + slash +
                list[7].ToString() + slash +
                list[6].ToString() + slash;
            for (int x = 8; x <= 15; x++)
            {
                strOctet += list[x].ToString() + slash;
            }

            return strOctet.Substring(0, strOctet.Length - 1).ToString();
        }
    }
}
