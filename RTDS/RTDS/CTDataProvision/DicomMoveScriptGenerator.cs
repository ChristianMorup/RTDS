using System.IO;
using System.Linq;
using System.Text;
using RTDS.Configuration;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;

//https://github.com/VarianAPIs/Varian-Code-Samples/blob/master/Eclipse%20Scripting%20API/plugins/GetDicomCollection.cs
namespace RTDS.CTDataProvision
{
    internal class DicomMoveScriptGenerator
    {
        private readonly string _aem = null;
        private readonly string _aec = null;
        private readonly string _aet = null;
        private readonly string _dcmtkBinPath = null;
        private readonly string _ipPort = null;

        public DicomMoveScriptGenerator()
        {
            var settings = ConfigurationManager.GetESAPISettings();
            _aem = settings.AEM;
            _aec = settings.AEC;
            _aet = settings.AET;
            _dcmtkBinPath = settings.DCMTK_BIN_PATH;
            _ipPort = settings.IP_PORT;
        }

        public void GenerateDicomMoveScript(Patient patient, PlanSetup plan, string filename)
        {
            string move = "movescu -v -aet " + _aet + " -aec " + _aec + " -aem " + _aem + " -S -k ";

            StreamWriter sw = new StreamWriter(filename, false, Encoding.ASCII);

            sw.WriteLine(@"@set PATH=%PATH%;" + _dcmtkBinPath);

            // write the command to move the 3D image data set
            if (plan.StructureSet != null && plan.StructureSet.Image != null)
            {
                sw.WriteLine("rem move 3D image " + plan.StructureSet.Image.Id);
                string cmd = move + '"' + "0008,0052=SERIES" + '"' + " -k " + '"' + "0020,000E=" +
                             plan.StructureSet.Image.Series.UID + '"' + _ipPort;
                sw.WriteLine(cmd);
            }

            // write the command to move the structure set
            if (plan.StructureSet != null)
            {
                sw.WriteLine("rem move StructureSet " + plan.StructureSet.Id);
                string cmd = move + '"' + "0008,0052=IMAGE" + '"' + " -k " + '"' + "0008,0018=" +
                             plan.StructureSet.UID + '"' + _ipPort;
                sw.WriteLine(cmd);
            }

            // write the command to move the plan
            {
                sw.WriteLine("rem move RTPlan " + plan.Id);
                string cmd = move + '"' + "0008,0052=IMAGE" + '"' + " -k " + '"' + "0008,0018=" + plan.UID + '"' +
                             _ipPort;
                sw.WriteLine(cmd);
            }

            // write the command to move all RT Dose objects (we can't tell from the scripting API which RTDose to use, send them all).
            foreach (Study study in patient.Studies)
            {
                if ((from s in study.Series where (s.Modality == SeriesModality.RTDOSE) select s).Any())
                {
                    sw.WriteLine("rem move all RTDose in study " + study.Id);
                    // Study instance UID and RTDoseStorage SOP Class UID
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
}