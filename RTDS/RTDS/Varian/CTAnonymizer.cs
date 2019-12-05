using System.IO;
using EvilDICOM.Anonymization.Settings;
using EvilDICOM.Core;

namespace RTDS.VarianAPI
{
    internal class CTAnonymizer
    {
        public static void AnonymizeCT(string filename)
        {
            var toAnonymize = Directory.GetFiles(filename);

            var settings = AnonymizationSettings.Default;
            //Change mapping but keep connections
            settings.DoAnonymizeUIDs = true;

            //Gets a current list of UIDs so it can create new ones 
            var queue = EvilDICOM.Anonymization.AnonymizationQueue.BuildQueue(settings, toAnonymize);

            foreach (var file in toAnonymize)
            {
                var dcm = DICOMObject.Read(file);
                queue.Anonymize(dcm);
                //Write back to initial location - though this can be a different place
                dcm.Write(file);
            }
        }
    }
}