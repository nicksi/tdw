//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18047
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace tdw
{
    
    internal partial class Resources
    {
        private static System.Resources.ResourceManager manager;
        internal static System.Resources.ResourceManager ResourceManager
        {
            get
            {
                if ((Resources.manager == null))
                {
                    Resources.manager = new System.Resources.ResourceManager("tdw.Resources", typeof(Resources).Assembly);
                }
                return Resources.manager;
            }
        }
        internal static Microsoft.SPOT.Font GetFont(Resources.FontResources id)
        {
            return ((Microsoft.SPOT.Font)(Microsoft.SPOT.ResourceUtility.GetObject(ResourceManager, id)));
        }
        internal static byte[] GetBytes(Resources.BinaryResources id)
        {
            return ((byte[])(Microsoft.SPOT.ResourceUtility.GetObject(ResourceManager, id)));
        }
        [System.SerializableAttribute()]
        internal enum BinaryResources : short
        {
            todotypes = -32514,
            _03hours = -27944,
            _09hours_o = -27915,
            _01hours = -25762,
            _10hours_o = -23500,
            _08hours = -18587,
            _09hours = -17754,
            _02hours_o = -14440,
            _04hours_o = -13241,
            _07hours_o2 = -13166,
            _03hours_o2 = -13134,
            _06hours_o = -11254,
            _04hours_o2 = -6979,
            _08hours_o2 = -6907,
            _02hours_o2 = -2973,
            _06hours_o2 = -2845,
            _05hours_o2 = -788,
            _01hours_o2 = -684,
            _09hours_o2 = -556,
            _12hours = 4654,
            _10hours_o2 = 5135,
            _10hours = 7072,
            _11hours_o2 = 7288,
            _08hours_o = 7893,
            _11hours = 8161,
            _11hours_o = 8663,
            _07hours_o = 10283,
            _05hours_o = 16486,
            _12hours_o = 19572,
            corner = 20297,
            _03hours_o = 23481,
            _07hours = 25300,
            _12hours_o2 = 25713,
            _04hours = 26393,
            _05hours = 27482,
            _02hours = 28319,
            _01hours_o = 29688,
            todotypes_inverted = 30983,
            _06hours = 32411,
        }
        [System.SerializableAttribute()]
        internal enum FontResources : short
        {
            ubuntu18c = -15004,
            ubuntu11c = -6229,
            ubuntu12c = -287,
            ubuntu16c = 23027,
            ubuntu15c = 30013,
        }
    }
}
