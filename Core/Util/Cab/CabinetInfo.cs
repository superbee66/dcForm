using System.Runtime.InteropServices;

namespace dCForm.Core.Util.Cab
{
    [StructLayout(LayoutKind.Sequential)]
    public class CabinetInfo //Cabinet API: "FDCABINETINFO"
    {
        public int cbCabinet;
        public short cFolders;
        public short cFiles;
        public short setID;
        public short iCabinet;
        public int fReserve;
        public int hasprev;
        public int hasnext;
    }
}