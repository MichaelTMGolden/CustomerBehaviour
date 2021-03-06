using CustomerBehaviour.Definitions;
using CustomerBehaviour.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CustomerBehaviour.Application
{
    public class NormalizedDataWriter : INormalizedDataWriter
    {
        public void WriteNormalizedDataToFile(CustomersBehaviourSnapshot snapshot)
        {
            //File Directory
            string _saveLocation = $"C:\\CustomerBehaviour\\Analysis\\Normalized-Data";
            //File Name 
            var filePath = $"{_saveLocation}\\{snapshot.name}";

            if (!Directory.Exists(_saveLocation))
                Directory.CreateDirectory(_saveLocation);

            if (!File.Exists(filePath))
            {

                //Creates File
                using FileStream fs = File.Create(filePath);
                var properties = typeof(Customer).GetProperties();
                var heading = "Target,Field1,Field2,Field3,Field4,Field5,Field6,Field7,Field8,Field9,Field10,Field11,Field12,Field13,Field14,Field15,Field16,Field17,Field18,Field19,Field20,Field21,Field22,Field23,Field24,Field25,Field26,Field27,Field28,Field29,Field30,Field31,Field32,Field33,Field34,Field35,Field36,Field37,Field38,Field39,Field40,Field41,Field42,Field43,Field44,Field45,Field46,Field47,Field48,Field49,Field50,Field51,Field52,Field53,Field54,Field55,Field56,Field57,Field58,Field59,Field60,Field61,Field62,Field63,Field64,Field65,Field66,Field67,Field68,Field69,Field70,Field71,Field72,Field73,Field74,Field75,Field76,Field77,Field78,Field79,Field80,Field81,Field82,Field83,Field84,Field85,Field86,Field87,Field88,Field89,Field90,Field91,Field92,Field93,Field94,Field95,Field96,Field97,Field98,Field99,Field100,Field101,Field102,Field103,Field104,Field105,Field106,Field107,Field108,Field109,Field110,Field111,Field112,Field113,Field114,Field115,Field116,Field117,Field118,Field119,Field120,Field121,Field122,Field123,Field124,Field125,Field126,Field127,Field128,Field129,Field130,Field131";
                byte[] headingByte = new UTF8Encoding(true).GetBytes(heading.ToString());
                fs.Write(headingByte);
                byte[] newline = Encoding.ASCII.GetBytes(Environment.NewLine);
                fs.Write(newline, 0, newline.Length);
                foreach (var customer in snapshot.customers)
                {
                    foreach (PropertyInfo property in properties)
                    {
                        var data = property.GetValue(customer);
                        byte[] info = new UTF8Encoding(true).GetBytes(data.ToString());
                        fs.Write(info, 0, info.Length);
                        var comma = ',';
                        byte[] commaByte = new UTF8Encoding(true).GetBytes(comma.ToString());
                        fs.Write(commaByte);
                    }

                    fs.SetLength(fs.Length - 1);
                    fs.Write(newline, 0, newline.Length);
                }

            }
        }
    }
}
