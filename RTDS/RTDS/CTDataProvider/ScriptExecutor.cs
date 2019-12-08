using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using RTDS.Configuration;
using RTDS.CTDataProvider.Callbacks;
using RTDS.CTDataProvider.Exceptions;
using RTDS.DTO;
using VMS.TPS.Common.Model.API;

//https://github.com/VarianAPIs/Varian-Code-Samples/blob/master/Eclipse%20Scripting%20API/plugins/GetDicomCollection.cs
namespace RTDS.CTDataProvider
{
    internal class ScriptExecutor
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private readonly StandardProcessErrorHandler _processErrorHandler = new StandardProcessErrorHandler();
        private readonly DicomMoveScriptGenerator _dicomMoveScriptGenerator = new DicomMoveScriptGenerator();

        private string GetTempStorage()
        {
            return ConfigurationManager.GetESAPISettings().TempStorage;
        }

        public void Execute(string patientId, string id, ICorrectedCTScanRetrievedCallback callback)
        {
            var app = Application.CreateApplication(null, null);
            Execute(app, patientId, id, callback);
        }

        public void Execute(Application app, string patientId, string id, ICorrectedCTScanRetrievedCallback callback)
        {
            var patient = GetPatient(app, patientId);
            var planSetup = GetPlanSetup(patient);
            var ctId = Guid.NewGuid().ToString();
            var cmdFilePath = CreateCmdFilePath(ctId);

            _dicomMoveScriptGenerator.GenerateDicomMoveScript(patient, planSetup, cmdFilePath);

            ExecuteCmdFile(cmdFilePath);
            var ctInfo = CreateCtScanInfoFromFiles(ctId);

            CTAnonymizer.AnonymizeCT(ctInfo);

            callback.OnCorrectedCTScanRetrieved(ctInfo, id);
        }

        public void Execute(string patientId, ICTScanRetrievedCallback callback)
        {
            var app = Application.CreateApplication(null, null);
            Execute(app, patientId, callback);
        }

        public void Execute(Application app, string patientId, ICTScanRetrievedCallback callback)
        {
            var patient = GetPatient(app, patientId);
            var planSetup = GetPlanSetup(patient);
            var ctId = Guid.NewGuid().ToString();
            var cmdFilePath = CreateCmdFilePath(ctId);

            _dicomMoveScriptGenerator.GenerateDicomMoveScript(patient, planSetup, cmdFilePath);

            ExecuteCmdFile(cmdFilePath);
            var ctInfo = CreateCtScanInfoFromFiles(ctId);

            CTAnonymizer.AnonymizeCT(ctInfo);

            callback.OnCTScanRetrieved(ctInfo);
        }

        private CTScanInfo CreateCtScanInfoFromFiles(string ctId)
        {
            var dcmFiles = Directory.GetFiles(GetTempStorage(), "*" + ctId + "*" + ".dcm");
            return new CTScanInfo(dcmFiles, ctId);
        }

        private string CreateCmdFilePath(string ctId)
        {
            var cmdFilename = @"move-" + ctId + ".cmd";
            return Path.Combine(GetTempStorage(), cmdFilename);
        }

        private Patient GetPatient(Application app, string patientId)
        {
            return app.OpenPatientById(patientId);
        }

        private PlanSetup GetPlanSetup(Patient patient)
        {
            var courses = patient.Courses;
            return courses.First().PlanSetups.First();
        }

        private void ExecuteCmdFile(string cmdFilePath)
        {
            string standardErrorsFromProcess;

            using (Process process = new Process())
            {
                string command = $@"&'{cmdFilePath}'";
                process.StartInfo.FileName = "PowerShell.exe";
                process.StartInfo.Arguments = command;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardError = true;

                process.ErrorDataReceived += new DataReceivedEventHandler(_processErrorHandler.HandleStandardError);

                process.Start();

                standardErrorsFromProcess = process.StandardError.ReadToEnd();
                process.WaitForExit();
                process.Close();
            }

            if (standardErrorsFromProcess.Length > 0)
            {
                Logger.Fatal("Failed to retrieve CT-scan: " + standardErrorsFromProcess);
                throw new CTProviderException(standardErrorsFromProcess);
            }
        }
    }
}