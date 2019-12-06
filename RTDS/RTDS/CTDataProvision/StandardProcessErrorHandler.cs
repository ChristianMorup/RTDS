using System;
using System.Diagnostics;
using System.Text;

//https://github.com/VarianAPIs/Varian-Code-Samples/blob/master/Eclipse%20Scripting%20API/plugins/GetDicomCollection.cs
namespace RTDS.CTDataProvision
{
    internal class StandardProcessErrorHandler
    {
        private readonly StringBuilder _stringBuilder = new StringBuilder("");

        public void HandleStandardError(object sendingProcess, DataReceivedEventArgs outline)
        {
            if (!String.IsNullOrEmpty(outline.Data))
            {
                _stringBuilder.Append(Environment.NewLine + outline.Data);
            }
        }
    }
}