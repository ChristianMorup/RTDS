using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using RTDS.Configuration;
using RTDS.CTDataProvider.Callbacks;
using RTDS.CTDataProvider.Exceptions;
using RTDS.DTO;
using RTDS.ExceptionHandling;
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

        public Task Execute(string patientId, string id, List<ICorrectedCTScanRetrievedCallback> callbacks)
        {
            var app = Application.CreateApplication(null, null);
            return Execute(app, patientId, id, callbacks);
        }

        public Task Execute(Application app, string patientId, string id, List<ICorrectedCTScanRetrievedCallback> callbacks)
        {
            return Task.Run(async () =>
            {
                var patient = await GetPatient(app, patientId);
                var planSetup = GetPlanSetup(patient);
                var ctId = Guid.NewGuid().ToString();
                var cmdFilePath = CreateCmdFilePath(ctId);

                await _dicomMoveScriptGenerator.GenerateDicomMoveScript(patient, planSetup, cmdFilePath);

                await ExecuteCmdFile(cmdFilePath);
                var ctInfo = await CreateCtScanInfoFromFiles(ctId);

                CTAnonymizer.AnonymizeCT(ctInfo);

                foreach (var callback in callbacks)
                {
                    TaskWatcher.AddTask(Task.Run(() => callback.OnCorrectedCTScanRetrieved(ctInfo, id)));
                }
            });
        }

        public Task Execute(string patientId, List<ICTScanRetrievedCallback> callbacks)
        {
            var app = Application.CreateApplication(null, null);
            return Execute(app, patientId, callbacks);
        }

        public Task Execute(Application app, string patientId, List<ICTScanRetrievedCallback> callbacks)
        {
            return Task.Run(async () =>
            {
                var patient = await GetPatient(app, patientId);
                var planSetup = GetPlanSetup(patient);
                var ctId = Guid.NewGuid().ToString();
                var cmdFilePath = CreateCmdFilePath(ctId);

                await _dicomMoveScriptGenerator.GenerateDicomMoveScript(patient, planSetup, cmdFilePath);

                await ExecuteCmdFile(cmdFilePath);
                
                var ctInfo = await CreateCtScanInfoFromFiles(ctId);

                CTAnonymizer.AnonymizeCT(ctInfo);

                foreach (var callback in callbacks)
                {
                    TaskWatcher.AddTask(Task.Run(() => callback.OnCTScanRetrieved(ctInfo)));
                }
            });
        }

        private Task<CTScanInfo> CreateCtScanInfoFromFiles(string ctId)
        {
            return Task.Run(() =>
            {
                var dcmFiles = Directory.GetFiles(GetTempStorage(), "*" + ctId + "*" + ".dcm");
                return new CTScanInfo(dcmFiles, ctId);
            });
        }

        private string CreateCmdFilePath(string ctId)
        {
            var cmdFilename = @"move-" + ctId + ".cmd";
            return Path.Combine(GetTempStorage(), cmdFilename);
        }

        private Task<Patient> GetPatient(Application app, string patientId)
        {
            return Task.Run(() => app.OpenPatientById(patientId));
        }

        private PlanSetup GetPlanSetup(Patient patient)
        {
            var courses = patient.Courses;
            return courses.First().PlanSetups.First();
        }

        private Task ExecuteCmdFile(string cmdFilePath)
        {
            return Task.Run(() =>
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
            });
        }
    }
}