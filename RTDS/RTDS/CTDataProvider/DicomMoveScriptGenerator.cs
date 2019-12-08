using System.IO;
using System.Linq;
using System.Text;
using RTDS.Configuration;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;

//https://github.com/VarianAPIs/Varian-Code-Samples/blob/master/Eclipse%20Scripting%20API/plugins/GetDicomCollection.cs
namespace RTDS.CTDataProvider
{
    internal class DicomMoveScriptGenerator
    {
        private readonly string _aem;
        private readonly string _aec;
        private readonly string _aet;
        private readonly string _dcmtkBinPath;
        private readonly string _ipPort;

        public DicomMoveScriptGenerator()
        {
            var settings = ConfigurationManager.GetESAPISettings();
            _aem = settings.AEM;
            _aec = settings.AEC;
            _aet = settings.AET;
            _dcmtkBinPath = settings.DcmtkBinPath;
            _ipPort = settings.IpPort;
        }

        public void GenerateDicomMoveScript(Patient patient, PlanSetup plan, string filename)
        {
            string move = "movescu -v -aet " + _aet + " -aec " + _aec + " -aem " + _aem + " -S -k ";

            using (StreamWriter sw = new StreamWriter(filename, false, Encoding.ASCII))
            {
                sw.WriteLine(@"@set PATH=%PATH%;" + _dcmtkBinPath);

                if (plan.StructureSet != null && plan.StructureSet.Image != null)
                {
                    AddCommandToMove3DImage(patient, plan, sw, filename);
                }
                if (plan.StructureSet != null)
                {
                    AddCommandToMoveStructureSet(plan, sw, move);
                }
                AddCommandToMovePlan(plan, sw, move);

                foreach (Study study in patient.Studies)
                {
                    if ((from s in study.Series where (s.Modality == SeriesModality.RTDOSE) select s).Any())
                    {
                        sw.WriteLine("rem move all RTDose in study " + study.Id);
                        string cmd =
                            move +
                            "\"0008,0052=IMAGE\" -k \"0008,0016=1.2.840.10008.5.1.4.1.1.481.2\" -k \"0020,000D=" +
                            study.UID + '"' + _ipPort;
                        sw.WriteLine(cmd);
                    }
                }

                sw.Flush();
                sw.Close();
            }
        }

        private void AddCommandToMove3DImage(Patient patient, PlanSetup plan, StreamWriter sw, string move)
        {
            sw.WriteLine("rem move 3D image " + plan.StructureSet.Image.Id);
            string cmd = move + '"' + "0008,0052=SERIES" + '"' + " -k " + '"' + "0020,000E=" +
                         plan.StructureSet.Image.Series.UID + '"' + _ipPort;
            sw.WriteLine(cmd);
        }
        
        private void AddCommandToMoveStructureSet(PlanSetup plan, StreamWriter sw, string move)
        {
            sw.WriteLine("rem move StructureSet " + plan.StructureSet.Id);
            string cmd = move + '"' + "0008,0052=IMAGE" + '"' + " -k " + '"' + "0008,0018=" +
                         plan.StructureSet.UID + '"' + _ipPort;
            sw.WriteLine(cmd);
        }
        
        private void AddCommandToMovePlan(PlanSetup plan, StreamWriter sw, string move)
        {
            sw.WriteLine("rem move RTPlan " + plan.Id);
            string cmd = move + '"' + "0008,0052=IMAGE" + '"' + " -k " + '"' + "0008,0018=" + plan.UID + '"' +
                         _ipPort;
            sw.WriteLine(cmd);
        }
    }
}