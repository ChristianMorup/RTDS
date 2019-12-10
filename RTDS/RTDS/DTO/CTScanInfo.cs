namespace RTDS.DTO
{
    public class CTScanInfo
    {
        public string[] DcmFiles { get; }

        public string Id { get; }

        public CTScanInfo(string[] dcmFiles, string id)
        {
            DcmFiles = dcmFiles;
            Id = id;
        }
    }
}
