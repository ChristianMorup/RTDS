using System.IO;
using EvilDICOM.Anonymization.Settings;
using EvilDICOM.Core;
using RTDS.DTO;

//http://rexcardan.github.io/Evil-DICOM/articles/anonymization.html
namespace RTDS.VarianAPI
{
    internal class CTAnonymizer
    {
        public static void AnonymizeCT(CTScanInfo ctInfo)
        {
            var settings = AnonymizationSettings.Default;
            //Change mapping but keep connections
            settings.DoAnonymizeUIDs = true;

            //Gets a current list of UIDs so it can create new ones 
            var queue = EvilDICOM.Anonymization.AnonymizationQueue.BuildQueue(settings, ctInfo.DcmFiles);

            foreach (var file in ctInfo.DcmFiles)
            {
                var dcm = DICOMObject.Read(file);
                queue.Anonymize(dcm);
                //Write back to initial location - though this can be a different place
                dcm.Write(file);
            }
        }
    }
}