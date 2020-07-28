using System;
using System.Collections.Generic;

namespace Stira.Socket.Services
{
    /// <summary>
    /// Simple conversions from one format to other
    /// </summary>
    public static class Conversion
    {
        /// <summary>
        /// Converts byte array to string array, normal conversions like ethernet packet to display
        /// able string
        /// </summary>
        /// <param name="byteArray">Input Byte array to be converted</param>
        /// <param name="sperator">Seprator string</param>
        /// <returns>String</returns>
        public static string ByteArrayToString(byte[] byteArray, string sperator = " ")
        {
            string hex = BitConverter.ToString(byteArray);
            return hex.Replace("-", sperator);
        }

        /// <summary>
        /// Converts byte array to string array, normal conversions like ethernet packet to display
        /// able string
        /// </summary>
        /// <param name="byteList">Input Byte array to be converted</param>
        /// <param name="sperator">Seprator string</param>
        /// <returns>String</returns>
        public static string ByteArrayToString(List<byte> byteList, string sperator = " ")
        {
            return ByteArrayToString(byteList?.ToArray(), sperator);
        }

        /// <summary>
        /// This method converts hex string to byte array e.g. "010203" or "0x01 0x02 0x03" =&gt;
        /// byte[3] with 01 02 03 values
        /// </summary>
        /// <param name="hex"></param>
        /// <returns>Byte Array</returns>
        public static byte[] StringToByteArray(string hex)
        {
            if (!string.IsNullOrEmpty(hex))
            {
                hex = hex.Replace(" ", ""); //Remove Spacing
                hex = hex.Replace("0x", ""); //Remove 0x for hex
                hex = hex.Replace("0X", ""); //Remove 0X for hex
                int NumberChars = hex.Length;
                byte[] bytes = new byte[NumberChars / 2];
                for (int i = 0; i < NumberChars; i += 2)
                {
                    bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
                }

                return bytes;
            }
            else
            {
                return null;
            }
        }
    }
}