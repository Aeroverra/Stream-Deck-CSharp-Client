using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tech.aerove.streamdeck.client.SDAnalyzer.ManifestModels;
using tech.aerove.streamdeck.client.SDAnalyzer.SettingsModels;

namespace tech.aerove.streamdeck.client.SDAnalyzer.CacheModels
{
    internal class CCache
    {
        public List<CProfile> Profiles = new List<CProfile>();

        public void Load(List<MProfile> dataProfiles)
        {
            foreach(var dataProfile in dataProfiles)
            {
                var profile = Profiles.SingleOrDefault(x=>x.UUID == dataProfile.UUID);
                if(profile == null)
                {
                    profile = new CProfile
                    {

                    };
                    Profiles.Add(profile);
                }
            }
        }
    }
}
