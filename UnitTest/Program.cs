using System.Data;

var dt = new DataTable();

dt.Columns.Add(new DataColumn("Col1", typeof(string)));
dt.Columns.Add(new DataColumn("Col2", typeof(int)));
dt.Columns.Add(new DataColumn("Col3", typeof(decimal)));

var dr0 = dt.NewRow();
dr0["Col1"] = "qwer";
dr0["Col2"] = 12;
dr0["Col3"] = 99.99m;
var dr1 = dt.NewRow();
dr1["Col1"] = System.DBNull.Value;
dr1["Col2"] = 10;
dr1["Col3"] = 9.99m;

dt.Rows.Add(dr0);
dt.Rows.Add(dr1);

var qqq = dt.GetColumnAsList<string>("Col1");

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