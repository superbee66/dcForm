using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Web.Security;
using dCForm.Client.Util;

namespace dCForm
{
    /// <summary>
    ///     Summary description for DocumentKey
    /// </summary>
    [Table("DocKey")]
    public class DocKey
    {
        [Key]
        [Column(Order = 1)]
        public int Id { get; set; }

        [Key]
        [MaxLength(50)]
        [Column(Order = 2)]
        public string KeyName { get; set; }

        [MaxLength(50)]
        [Column(Order = 3)]
        [Required]
        public string KeyVal { get; set; }

        /// <summary>
        /// </summary>
        /// <param name="DocKeys"></param>
        /// <param name="ClearText">don't Rijndael encrypt</param>
        /// <returns>A jsoned & modified base64 string suitable for parameter UrlEncoding</returns>
        public static string DocIdFromKeys(Dictionary<string, string> DocKeys, bool ClearText = false) =>
            ClearText
                ? Serialize.Json.Serialize(DocKeys)
                : MachineKey.Encode(
                    Encoding.UTF8.GetBytes(
                        Serialize.Json.Serialize(DocKeys)),
                    MachineKeyProtection.Encryption);

        public static Dictionary<string, string> DocIdToKeys(string DocId) =>
            string.IsNullOrWhiteSpace(DocId)
                ? new Dictionary<string, string>()
                : Serialize.Json.Deserialize<Dictionary<string, string>>(
                    Encoding.UTF8.GetString(
                        MachineKey.Decode(DocId, MachineKeyProtection.Encryption)));
    }
}