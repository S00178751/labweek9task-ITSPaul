using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Shared.Protocol;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace ActivityTracker
{
    public static class Activity
    {
        public class StudentEntity : TableEntity
        {
            public string ExamName { get; set; } = ConfigurationSettings.AppSettings["ExamName"];
            public string ip { get; set; } = GetLocalIPAddress();
            public string Name { get; set; }
            public string CopyId { get; set; } = LabStarter.Properties.Settings.Default["CopyId"].ToString();
            public StudentEntity() { }
            public StudentEntity(string studentID, string Name)
            {
                this.PartitionKey = studentID;
                this.RowKey = Guid.NewGuid().ToString();
                this.Name = Name;
            }

        }
        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("Local IP Address Not Found!");
        }

        private static void recordLocalActivity(StudentEntity studentRec)
            {

            XmlSerializer serializer =
                new XmlSerializer(typeof(StudentEntity));
            
            // Make sure the output file exists
            if (!File.Exists(@".\StudentData\Data.xml"))
            {
                File.CreateText(@".\StudentData\Data.xml");
            }

            using (Stream fs = new FileStream(@".\StudentData\Data.xml",FileMode.Append))
                    {
                        XmlWriter writer =
                        new XmlTextWriter(fs, Encoding.UTF8);
                        // Serialize using the XmlTextWriter.
                        serializer.Serialize(writer, studentRec);
                        writer.WriteString(Environment.NewLine);
                        writer.Close();
                    }

            }

    public static int RunCount()
        {
            string studentID = ConfigurationSettings.AppSettings["StudentId"]; ;
            int noOfRuns = 0;
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                    //CloudConfigurationManager.GetSetting("StorageConnectionString")
                    ConfigurationSettings.AppSettings["StorageConnectionString"]
                    );
            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            CloudTable table = null;
            string tableReference = ConfigurationSettings.AppSettings["ExamName"];
            table = tableClient.GetTableReference(tableReference);
            if (tableClient.ListTables(tableReference).Count() > 0)
            {
                noOfRuns = (from student in table.CreateQuery<StudentEntity>()
                            where student.PartitionKey == studentID
                            select student).ToList().Count();
            }
            return noOfRuns;
        }

        public static void track()
        {
            try
            {
                if(LabStarter.Properties.Settings.Default["CopyId"] == string.Empty)
                {
                    LabStarter.Properties.Settings.Default["CopyId"] = Guid.NewGuid().ToString();
                    LabStarter.Properties.Settings.Default.Save();
                }

                if (LabStarter.Properties.Settings.Default["IpAddress"] == string.Empty)
                {
                    LabStarter.Properties.Settings.Default["IpAddress"] = GetLocalIPAddress().ToString();
                    LabStarter.Properties.Settings.Default.Save();
                }

                // Parse the connection string and return a reference to the storage account.
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                    //CloudConfigurationManager.GetSetting("StorageConnectionString")
                    ConfigurationSettings.AppSettings["StorageConnectionString"]
                    );
                // Create the table client.
                CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

                CloudTable table = null;

                string tableReference = ConfigurationSettings.AppSettings["ExamName"];
                table = tableClient.GetTableReference(tableReference);
                // Retrieve a reference to the table.
                if (tableClient.ListTables(tableReference).Count() < 1)
                {
                    // Create the table if it doesn't exist.
                    table.CreateIfNotExists();
                }
                string studentID = ConfigurationSettings.AppSettings["StudentId"];
                string studentName = ConfigurationSettings.AppSettings["StudentName"];
                
                StudentEntity student = new StudentEntity(studentID, studentName);
                // Create the TableOperation object that inserts the student entity.
                TableOperation insertOperation = TableOperation.Insert(student);
                // Execute the insert operation.
                var result = table.Execute(insertOperation);
                recordLocalActivity(student);
                Debug.WriteLine(result.Result);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(" Exception " + ex.Message);
            }
        }


    }
}

