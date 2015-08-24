using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using System.IO;
using System.Web;
using System.ServiceModel;
using JSONEpicor.UD04;
using JSONEpicor.UD100;

namespace JSONEpicor
{
    class Program
    {
        static void Main(string[] args)
        {
            //part attributes
            /*using (TextReader reader = File.OpenText(@"Part.json"))
            //using (TextReader reader = File.OpenText(@"C:\Users\Bob\Documents\Visual Studio 2013\Projects\Kaufman SalesRFQ\Kaufman SalesRFQ\SalesRFQ.App\DataModel\Part.json"))
            {
                string readFile = reader.ReadToEnd();
                dynamic json = JsonConvert.DeserializeObject(readFile);

                UD04SvcContractClient ud04 = InitUD04Client();
                GetNewUD04(ud04, json);

                Console.WriteLine("Import to UD04 done!");
                Console.ReadLine();
            }*/

            //attributes
            //using (TextReader reader = File.OpenText(@"C:\Users\Bob\Documents\Visual Studio 2013\Projects\Kaufman SalesRFQ\Kaufman SalesRFQ\SalesRFQ.App\DataModel\Attribute.json"))
            using (TextReader reader = File.OpenText(@"Attributes.json"))
            {
                string readFile = reader.ReadToEnd();
                dynamic json = JsonConvert.DeserializeObject(readFile);

                UD100SvcContractClient ud100 = InitUD100Client();
                GetNewUD100(ud100, json);

                Console.WriteLine("Import to UD100 done!");
                Console.ReadLine();                                
            }
        }

        private static UD04SvcContractClient InitUD04Client()
        {
            System.Net.ServicePointManager.ServerCertificateValidationCallback = ((sender, certificate, chain, sslPolicyErrors) => true);

            BasicHttpBinding binding = new BasicHttpBinding();
            binding.Security.Mode = BasicHttpSecurityMode.TransportWithMessageCredential;

            binding.MaxReceivedMessageSize = 999999999;

            EndpointAddress remoteAddress = new EndpointAddress("https://epicor/EpicorTest2/Ice/BO/UD04.svc");
            //EndpointAddress remoteAddress = new EndpointAddress("https://dev-epicor10.saberlogicllc.local/ERP100700_RS/Ice/BO/UD04.svc");
            UD04SvcContractClient ud04 = new UD04.UD04SvcContractClient(binding, remoteAddress);

            ud04.ClientCredentials.UserName.UserName = "manager";
            //ud04.ClientCredentials.UserName.Password = "manager";
            ud04.ClientCredentials.UserName.Password = "Epicor2015";

            return ud04;
        }

        private static UD100SvcContractClient InitUD100Client()
        {
            System.Net.ServicePointManager.ServerCertificateValidationCallback = ((sender, certificate, chain, sslPolicyErrors) => true);

            BasicHttpBinding binding = new BasicHttpBinding();
            binding.Security.Mode = BasicHttpSecurityMode.TransportWithMessageCredential;

            binding.MaxReceivedMessageSize = 999999999;

            EndpointAddress remoteAddress = new EndpointAddress("https://epicor/EpicorTest2/Ice/BO/UD100.svc");
            //EndpointAddress remoteAddress = new EndpointAddress("https://dev-epicor10.saberlogicllc.local/ERP100700_RS/Ice/BO/UD100.svc");
            UD100SvcContractClient ud100 = new UD100SvcContractClient(binding, remoteAddress);

            ud100.ClientCredentials.UserName.UserName = "manager";
            //ud100.ClientCredentials.UserName.Password = "manager";
            ud100.ClientCredentials.UserName.Password = "Epicor2015";

            return ud100;            
        }

        private static void GetNewUD04(UD04SvcContractClient ud04, dynamic json)
        {
            for (int x = 0; x < json.Count; x++)
            {
                for (int y = 0; y < json[x]["ATTRIBUTES"].Count; y++)
                {
                    string partNum = json[x]["PART_DESCRIPTION"];
                    string attrID = json[x]["ATTRIBUTES"][y]["ATTRIBUTE_ID"];
                    
                    UD04Tableset ud04DS = new UD04Tableset();
                    GetaNewUD04Request ud04Request = new GetaNewUD04Request(ud04DS);
                    GetaNewUD04Response ud04Response = ud04.GetaNewUD04Async(ud04Request).Result;

                    ud04Response.ds.UD04[0].Key1 = partNum;
                    ud04Response.ds.UD04[0].Key2 = attrID;
                    ud04Response.ds.UD04[0].Key3 = "";
                    ud04Response.ds.UD04[0].Key4 = "";
                    ud04Response.ds.UD04[0].Key5 = "";                    

                    ud04Response.ds.UD04[0].Character01 = attrID.Substring(4, attrID.Length - 4).Replace('_', ' ');

                    UD04.UpdateRequest ud04UpdateRequest = new UD04.UpdateRequest(ud04Response.ds);
                    UD04.UpdateResponse ud04UpdateResponse = new UD04.UpdateResponse(ud04Response.ds);

                    ud04UpdateResponse = ud04.UpdateAsync(ud04UpdateRequest).Result;
                }
            }
        }

        private static void GetNewUD100(UD100SvcContractClient ud100, dynamic json)
        {
            for(int x = 0; x < json.Count; x++)
            {
                UD100Tableset ud100DS = new UD100Tableset();
                GetaNewUD100Request ud100Request = new GetaNewUD100Request(ud100DS);
                GetaNewUD100Response ud100Response = ud100.GetaNewUD100Async(ud100Request).Result;

                ud100Response.ds.UD100[0].Key1 = json[x]["ATTRIBUTE_ID"];
                ud100Response.ds.UD100[0].Character01 = json[x]["ATTRIBUTE_DESCRIPTION"];
                ud100Response.ds.UD100[0].ShortChar01 = (json[x]["SELECTION_MODE"] == null ? "" : json[x]["SELECTION_MODE"]);
                ud100Response.ds.UD100[0].CheckBox01 = (json[x]["DISPLAY_IN_TITLE"] == null ? false : json[x]["DISPLAY_IN_TITLE"]);
                ud100Response.ds.UD100[0].ShortChar02 = (json[x]["TITLE_PREFIX"] == null ? "" : json[x]["TITLE_PREFIX"]);
                ud100Response.ds.UD100[0].Character02 = "";
                ud100Response.ds.UD100[0].CheckBox02 = (json[x]["MANDATORY"] == null ? false : true);
                ud100Response.ds.UD100[0].CheckBox03 = true;

                UD100.UpdateRequest ud100UpdateRequest = new UD100.UpdateRequest(ud100DS);
                UD100.UpdateResponse ud100UpdateResponse = new UD100.UpdateResponse(ud100DS);

                ud100UpdateResponse = ud100.UpdateAsync(ud100UpdateRequest).Result;

                /*
                UD100.Key1 = Attribute ID
                UD100.Character01 = Attribute Description
                UD100.ShortChar01 = Selection Mode
                UD100.CheckBox01 = Display Title
                UD100.ShortChar02 = Title Prefix
                UD100.Character02 = Image Path
                UD100.CheckBox02 = Mandatory
                UD100.CheckBox03 = Part
                UD100.CheckBox04 = Quote
                */
            }
        }
    }
}
