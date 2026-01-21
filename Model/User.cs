using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Applet.Nat.Ux.Models
{
    public class User
    {
        public UserModel? ioDcModel { get; set; }
        public eTask ieTask { get; set; }
        public string? ivstrPass { get; set; }
        public List<UserCuitModel> coCuitsModels { get; set; }
        public string? ivstrCuits { get; set; }
        public long ivlngCurrentCuit { get; set; }
        public void GetStrCuit()
        {
            ivstrCuits = string.Empty;
            if (this.coCuitsModels == null) 
                return;
            List<string> lcvstr = new List<string>();
            foreach (UserCuitModel lioO in coCuitsModels)
                if (lioO.ivblnDefaut)
                    lcvstr.Add($"[{lioO.ivlngCuit.ToString()}]");
                else
                    lcvstr.Add(lioO.ivlngCuit.ToString());
            ivstrCuits = string.Join(", ", lcvstr);
        }
        public void SetStrCuit()
        {
            coCuitsModels= new List<UserCuitModel>();
            UserCuitModel lioUserCuitModel;
            string livstrCuit;
            foreach (string livstr in ivstrCuits.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                livstrCuit=livstr.Trim();
                lioUserCuitModel = new UserCuitModel { ivnumUser = ioDcModel.ivnumUser };
                if (livstrCuit.StartsWith("[") && livstrCuit.EndsWith("]"))
                    lioUserCuitModel.ivblnDefaut = true;
                if (long.TryParse(livstrCuit.Replace("[", string.Empty).Replace("]", string.Empty), out long livlngCuit))
                {
                    lioUserCuitModel.ivlngCuit = livlngCuit;
                    coCuitsModels.Add(lioUserCuitModel);
                }
            }
        }
    }
    public class UserModel
    {
        public int ivnumUser { get; set; }
        public string? ivstrUserEmail { get; set; }
        public string? ivstrUserId { get; set; }
        public string? ivstrUserName { get; set; }
        public bool ivblnAdmin { get; set; }
        public bool ivblnEnable { get; set; }
        public short? ivnrologonFails { get; set; }
    }
    public class UserCuitModel
    {
        public int ivnumUser { get; set; }
        public long ivlngCuit { get; set; }
        public bool ivblnDefaut { get; set; }
        public string? ivstrRS { get; set; }
        public string? ivstrEncoding { get; set; }
    }
}
