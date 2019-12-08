using System;
using System.Collections.Generic;

namespace Stira.Socket.Services
{
    public static class Conversion
    {
        /// <summary>
        /// Converts byte array to string array, normal conversions like ethernet packet to display
        /// able string
        /// </summary>
        /// <param name="byteArray">Input Byte array to be converted</param>
        /// <param name="sperator">Seprator string</param>
        /// <returns>String</returns>
        public static string ByteArrayToString(byte[] byteArray, string seprator = " ")
        {
            string hex = BitConverter.ToString(byteArray);
            return hex.Replace("-", seprator);
        }

        /// <summary>
        /// Converts byte array to string array, normal conversions like ethernet packet to display
        /// able string
        /// </summary>
        /// <param name="byteList">Input Byte array to be converted</param>
        /// <param name="sperator">Seprator string</param>
        /// <returns>String</returns>
        public static string ByteArrayToString(List<byte> byteList, string seprator = " ")
        {
            return ByteArrayToString(byteList?.ToArray(), seprator);
        }
    }
}