using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

// ReSharper disable once CheckNamespace UnusedType.Global
[SuppressMessage("Design", "CA1050:Declare types in namespaces")]
public static class Extensions
{
    #region String Extensions
    
    /// <summary>
    /// Returns whether or not this string is null or empty.
    /// </summary>
    /// <param name="value">This string.</param>
    /// <returns>bool</returns>
    public static bool IsNullOrEmpty(this string value)
    {
        var flag = string.IsNullOrEmpty(value);
        return flag;
    }

    /// <summary>
    /// Returns whether or not this string is null, empty, or white space.
    /// </summary>
    /// <param name="value">This string.</param>
    /// <returns>bool</returns>
    public static bool IsNullOrWhiteSpace(this string value)
    {
        var flag = string.IsNullOrWhiteSpace(value);
        return flag;
    }

    /// <summary>
    /// Converts this string to a short.
    /// </summary>
    /// <param name="value">This string.</param>
    /// <returns>short</returns>
    public static short ToShort(this string value)
    {
        var integer = Convert.ToInt16(value);
        return integer;
    }

    /// <summary>
    /// Converts this string to an integer.
    /// </summary>
    /// <param name="value">This string.</param>
    /// <returns>short</returns>
    public static int ToInt(this string value)
    {
        var integer = Convert.ToInt32(value);
        return integer;
    }

    /// <summary>
    /// Converts this string to a long integer.
    /// </summary>
    /// <param name="value">This string.</param>
    /// <returns>short</returns>
    public static long ToLong(this string value)
    {
        var integer = Convert.ToInt64(value);
        return integer;
    }

    /// <summary>
    /// Converts this string to a decimal.
    /// </summary>
    /// <param name="value">This string.</param>
    /// <returns>short</returns>
    public static decimal ToDecimal(this string value)
    {
        var dec = Convert.ToDecimal(value);
        return dec;
    }

    /// <summary>
    /// Converts this string to a double.
    /// </summary>
    /// <param name="value">This string.</param>
    /// <returns>short</returns>
    public static double ToDouble(this string value)
    {
        var dbl = Convert.ToDouble(value);
        return dbl;
    }

    /// <summary>
    /// Converts this string to a date time.
    /// </summary>
    /// <param name="value">This string.</param>
    /// <param name="format">The particular format to parse for.</param>
    /// <returns>short</returns>
    public static DateTime ToDateTime(this string value, string format = "")
    {
        var dt = format == string.Empty
            ? DateTime.Parse(value)
            : DateTime.ParseExact(value, format, CultureInfo.InvariantCulture);
        return dt;
    }
    
    /// <summary>
    /// Encodes this string to a byte array with the specified encoding. 
    /// </summary>
    /// <param name="value">This string.</param>
    /// <param name="encoder">The type of encoding to perform. (BigEndianUnicode excluded)</param>
    /// <returns>byte[]</returns>
    public static IEnumerable<byte> ToBytes(this string value, Encoding? encoder = null!)
    {
        encoder ??= Encoding.Default;
        
        var bytes = encoder.ToString() switch
        {
            "System.Text.ASCIIEncoding+ASCIIEncodingSealed" => Encoding.ASCII.GetBytes(value),
            "System.Text.UTF8Encoding+UTF8EncodingSealed" => Encoding.UTF8.GetBytes(value),
            "System.Text.UTF32Encoding" => Encoding.UTF32.GetBytes(value),
            "System.Text.UnicodeEncoding" => Encoding.Unicode.GetBytes(value),
            "System.Text.Latin1Encoding+Latin1EncodingSealed" => Encoding.Latin1.GetBytes(value),
            _ => Encoding.Default.GetBytes(value)
        };

        return bytes;
    }

    /// <summary>
    /// Masks this string by returning a sha256 hash string.
    /// </summary>
    /// <param name="value">This string.</param>
    /// <returns>short</returns>
    public static string Mask(this string value)
    {
        var seed = Encoding.UTF8.GetBytes(value);
        var rgbCounter = new byte[4];
        var targetRgb = new byte[value.Length];
        uint counter = 0;
        for (var inc = 0; inc < targetRgb.Length;)
        {
            ConvertIntToByteArray(counter++, ref rgbCounter);
            var hash = (HashAlgorithm)CryptoConfig.CreateFromName("SHA256")!;
            var temp = new byte[4 + seed.Length];
            Buffer.BlockCopy(rgbCounter, 0, temp, 0, 4);
            Buffer.BlockCopy(seed, 0, temp, 4, seed.Length);
            hash.ComputeHash(temp);
            if (targetRgb.Length - inc > hash.HashSize / 8)
            {
                Buffer.BlockCopy(hash.Hash!, 0, targetRgb, inc, hash.Hash!.Length);
            }
            else
            {
                Buffer.BlockCopy(hash.Hash!, 0, targetRgb, inc, targetRgb.Length - inc);
            }

            inc += hash.Hash!.Length;
        }

        var text = Convert.ToBase64String(targetRgb);

        while (text.EndsWith("="))
        {
            text = text[..^1];
        }

        return text;
    } // Convert the specified integer to the byte array.

    /// <summary>
    /// Escapes characters in this string that have special meaning in Json.
    /// </summary>
    /// <param name="value">This string.</param>
    /// <returns>string</returns>
    public static string EscapeForJson(this string value)
    {
        var escaped = HttpUtility.JavaScriptStringEncode(value);
        return escaped;
    }

    /// <summary>
    /// Escapes characters in this string that have special meaning in Sql.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string EscapeForSql(this string value)
    {
        var text = Regex.Replace(value, @"[\x00'""\b\n\r\t\cZ\\%_]",
            delegate(Match match)
            {
                var v = match.Value;
                return v switch
                {
                    "\x00" => // ASCII NUL (0x00) character
                        "\\0",
                    "\b" => // BACKSPACE character
                        "\\b",
                    "\n" => // NEWLINE (linefeed) character
                        "\\n",
                    "\r" => // CARRIAGE RETURN character
                        "\\r",
                    "\t" => // TAB
                        "\\t",
                    "\u001A" => // Ctrl-Z
                        "\\Z",
                    _ => "\\" + v
                };
            });

        return text;
    }

    #endregion

    #region IEnumerable<byte> Extensions
    
    public static string ToByteString(this IEnumerable<byte> value, Encoding? encoder = null)
    {
        encoder ??= Encoding.Default;
        
        var text = encoder.ToString() switch
        {
            "System.Text.ASCIIEncoding+ASCIIEncodingSealed" => Encoding.ASCII.GetString((byte[])value),
            "System.Text.UTF8Encoding+UTF8EncodingSealed" => Encoding.UTF8.GetString((byte[])value),
            "System.Text.UTF32Encoding" => Encoding.UTF32.GetString((byte[])value),
            "System.Text.UnicodeEncoding" => Encoding.Unicode.GetString((byte[])value),
            "System.Text.Latin1Encoding+Latin1EncodingSealed" => Encoding.Latin1.GetString((byte[])value),
            _ => Encoding.Default.GetString((byte[])value)
        };

        return text;
    }
    
    #endregion

    #region Helper Methods
    
    private static void ConvertIntToByteArray(uint sourceInt, ref byte[] targetBytes)
    {
        var inc = 0; // Clear the array prior to filling it.
        Array.Clear(targetBytes, 0, targetBytes.Length);
        while (sourceInt > 0)
        {
            var remainder = sourceInt % 256;
            targetBytes[3 - inc] = (byte)remainder;
            sourceInt = (sourceInt - remainder) / 256;
            inc++;
        }
    }
    
    #endregion
}