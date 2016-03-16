using System.Text.RegularExpressions;
using Newtonsoft.Json.Serialization;

namespace BogdanM.LocationServices.GoogleMaps.Infrastructure
{
    public class SnakeCasePropertyNamesContractResolver : DefaultContractResolver
    {
        protected override string ResolvePropertyName(string propertyName)
        {
            var startUnderscores = Regex.Match(propertyName, @"^_+");
            var result = startUnderscores + Regex
                .Replace(propertyName, @"([A-Z])", "_$1").ToLower().TrimStart('_');

            return result;
        }
    }
}
