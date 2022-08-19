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

foreach (var item in t)
{
    Console.WriteLine(item.ToString());
}

Console.ReadLine();