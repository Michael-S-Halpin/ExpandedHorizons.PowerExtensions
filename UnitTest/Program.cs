var t = new object[13];

t[0] = " ".IsNullOrWhiteSpace();
t[1] = string.Empty.IsNullOrEmpty();
t[2] = "1".ToShort();
t[3] = "1".ToInt();
t[4] = "1".ToLong();
t[5] = "1.5".ToDecimal();
t[6] = "1".ToDouble();
t[7] = "01/01/2022".ToDateTime();
t[8] = "Make me \"JSON\" friendly.".EscapeForJson();
t[9] = "Make me 'SQL' friendly.".EscapeForSql();
t[10] = "Mask me.".Mask();
t[11] = "Byte me!".ToBytes();
t[12] = ((byte[])t[11]).ToByteString();

var q0 = new List<string>(new[] { "one", "two", "three" });
var q1 = new List<int>(new[] { 1, 2, 3 });
var q2 = new List<decimal>(new[] { 1.1m, 2.2m, 3.3m });

var w0 = q0.ToJson();
var w1 = q1.ToJson();
var w2 = q2.ToJson();

var e0 = w0.IsValidJson();
var e1 = "{Not:\"Valid\"}".IsValidJson();

foreach (var item in t)
{
    Console.WriteLine(item.ToString());
}

Console.ReadLine();