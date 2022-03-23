// See https://aka.ms/new-console-template for more information

using System.Security.Cryptography;
using System.Text;
using Trace;

Console.WriteLine("Hello, world!");

var img = new HdrImage(0, 0);

//(int a, int b) = img.parse_img_size("ss");



img.parse_img_size("-1 8");
