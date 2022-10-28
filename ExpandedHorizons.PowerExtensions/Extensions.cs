using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;

// ReSharper disable once CheckNamespace UnusedType.Global
[SuppressMessage("Design", "CA1050:Declare types in namespaces")]
public static class Extensions
{
    #region String Extensions

    /// <summary>
    /// Returns true if this string is a valid DateTime.
    /// </summary>
    /// <param name="value">This string</param>
    /// <returns>bool</returns>
    public static bool IsDateTime(this string value)
    {
        var flag = DateTime.TryParse(value, out _);
        return flag;
    }
    
    /// <summary>
    /// Returns true if this string is numeric, including formatted numbers with commas and currency signs.
    /// </summary>
    /// <param name="value">This string</param>
    /// <returns>bool</returns>
    public static bool IsNumeric(this string value)
    {
        var flag = Information.IsNumeric(value);
        return flag;
    }
    
    /// <summary>
    /// Returns true of this string is a strict number.
    /// </summary>
    /// <param name="value">This string</param>
    /// <returns>bool</returns>
    public static bool IsNumber(this string value)
    {
        var flag = double.TryParse(value, out _);
        return flag;
    }
    
    /// <summary>
    /// Remove all instances of text in this string.
    /// </summary>
    /// <param name="value">This string</param>
    /// <param name="text">The string to remove.</param>
    /// <returns>string</returns>
    public static string Remove(this string value, string text)
    {
        var txt = value.Replace(text, string.Empty);
        return txt;
    }

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
    /// <returns>bool</returns>
    public static bool ToBool(this string value)
    {
        var boolean = Convert.ToBoolean(value);
        return boolean;
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
        if (string.IsNullOrEmpty(value))
        {
            return value;
        }

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
    }

    /// <summary>
    /// Returns true if this string is a properly formatted Json string.
    /// TODO: Needs improvement this should be a strong check and the dependency does error correction.
    /// </summary>
    /// <param name="value">This string</param>
    /// <returns>bool</returns>
    public static bool IsValidJson(this string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }
        
        value = value.Trim();
        if ((!value.StartsWith("{") || !value.EndsWith("}")) && //For object
            (!value.StartsWith("[") || !value.EndsWith("]"))) return false; //For array
        
        try
        {
            var settings = new JsonLoadSettings
            {
                DuplicatePropertyNameHandling = DuplicatePropertyNameHandling.Error
            };
            var obj = JToken.Parse(value, settings);

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            return false;
        }
    }
    
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
    
    #region Object Extensions

    /// <summary>
    /// Returns true if this object is a valid DateTime.
    /// </summary>
    /// <param name="value">This string</param>
    /// <returns>bool</returns>
    public static bool IsDateTime(this object value)
    {
        return value.ToString()!.IsDateTime();
    }
    
    /// <summary>
    /// Returns true if this object is numeric, including formatted numbers with commas and currency signs.
    /// </summary>
    /// <param name="value">This string</param>
    /// <returns>bool</returns>
    public static bool IsNumeric(this object value)
    {
        return value.ToString()!.IsNumeric();
    }
    
    /// <summary>
    /// Returns true of this object is a strict number.
    /// </summary>
    /// <param name="value">This string</param>
    /// <returns>bool</returns>
    public static bool IsNumber(this object value)
    {
        return value.ToString()!.IsNumber();
    }
    
    /// <summary>
    /// Remove all instances of text in this object.
    /// </summary>
    /// <param name="value">This string</param>
    /// <param name="text">The string to remove.</param>
    /// <returns>string</returns>
    public static string Remove(this object value, string text)
    {
        return value.ToString()!.Remove(text);
    }

    /// <summary>
    /// Returns whether or not this object is null or empty.
    /// </summary>
    /// <param name="value">This string.</param>
    /// <returns>bool</returns>
    public static bool IsNullOrEmpty(this object value)
    {
        return value.ToString()!.IsNullOrEmpty();
    }

    /// <summary>
    /// Returns whether or not this object is null, empty, or white space.
    /// </summary>
    /// <param name="value">This string.</param>
    /// <returns>bool</returns>
    public static bool IsNullOrWhiteSpace(this object value)
    {
        return value.ToString()!.IsNullOrWhiteSpace();
    }

    /// <summary>
    /// Converts this object to a boolean.
    /// </summary>
    /// <param name="value">This string.</param>
    /// <returns>short</returns>
    public static bool ToBool(this object value)
    {
        return value.ToString()!.ToBool();
    }

    /// <summary>
    /// Converts this object to a short.
    /// </summary>
    /// <param name="value">This string.</param>
    /// <returns>short</returns>
    public static short ToShort(this object value)
    {
        return value.ToString()!.ToShort();
    }

    /// <summary>
    /// Converts this object to an integer.
    /// </summary>
    /// <param name="value">This string.</param>
    /// <returns>short</returns>
    public static int ToInt(this object value)
    {
        return value.ToString()!.ToInt();
    }

    /// <summary>
    /// Converts this object to a long integer.
    /// </summary>
    /// <param name="value">This string.</param>
    /// <returns>short</returns>
    public static long ToLong(this object value)
    {
        return value.ToString()!.ToLong();
    }

    /// <summary>
    /// Converts this object to a decimal.
    /// </summary>
    /// <param name="value">This string.</param>
    /// <returns>short</returns>
    public static decimal ToDecimal(this object value)
    {
        return value.ToString()!.ToDecimal();
    }

    /// <summary>
    /// Converts this object to a double.
    /// </summary>
    /// <param name="value">This string.</param>
    /// <returns>short</returns>
    public static double ToDouble(this object value)
    {
        return value.ToString()!.ToDouble();
    }

    /// <summary>
    /// Converts this object to a date time.
    /// </summary>
    /// <param name="value">This string.</param>
    /// <param name="format">The particular format to parse for.</param>
    /// <returns>short</returns>
    public static DateTime ToDateTime(this object value, string format = "")
    {
        return value.ToString()!.ToDateTime(format);
    }

    /// <summary>
    /// Encodes this object to a byte array with the specified encoding. 
    /// </summary>
    /// <param name="value">This string.</param>
    /// <param name="encoder">The type of encoding to perform. (BigEndianUnicode excluded)</param>
    /// <returns>byte[]</returns>
    public static IEnumerable<byte> ToBytes(this object value, Encoding? encoder = null!)
    {
        return value.ToString()!.ToBytes(encoder);
    }

    /// <summary>
    /// Masks this object by returning a sha256 hash string.
    /// </summary>
    /// <param name="value">This string.</param>
    /// <returns>short</returns>
    public static string Mask(this object value)
    {
        return value.ToString()!.Mask();
    } 

    /// <summary>
    /// Returns true if this object is a properly formatted Json string.
    /// TODO: Needs improvement this should be a strong check and the dependency does error correction.
    /// </summary>
    /// <param name="value">This string</param>
    /// <returns>bool</returns>
    public static bool IsValidJson(this object value)
    {
        return value.ToString()!.IsValidJson();
    }
    
    /// <summary>
    /// Escapes characters in this object that have special meaning in Json.
    /// </summary>
    /// <param name="value">This string.</param>
    /// <returns>string</returns>
    public static string EscapeForJson(this object value)
    {
        return value.ToString()!.EscapeForJson();
    }

    /// <summary>
    /// Escapes characters in this object that have special meaning in Sql.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string EscapeForSql(this object value)
    {
        return value.ToString()!.EscapeForSql();
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

    public static string GetDominantValue(this IEnumerable<string> value)
    {
        var dominant = value
            .GroupBy(i => i)
            .OrderByDescending(g => g.Count())
            .Select(g => g.Key)
            .First();
        return dominant;
    }
    
    #endregion
    
    #region JSON Methods

    /// <summary>
    /// Returns a json string from this serialized object.
    /// </summary>
    /// <param name="value">This object</param>
    /// <typeparam name="T">This type.</typeparam>
    /// <returns>string</returns>
    public static string ToJson<T>(this T value) //where T : IComparable<T>
    {
        var json = JsonConvert.SerializeObject(value);
        return json;
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