using System.Linq;
using BogdanM.LocationServices.Core;

namespace BogdanM.LocationServices.GoogleMaps.Dtos
{
    public class GoogleMapsGeocodingResponse
    {
        public GoogleMapsGeocoding[] Results { get; set; }
        public string Status { get; set; }

        public string StreetName => this.GetAddressComponent("route");
        public string StreetNo => this.GetAddressComponent("street_number");

        public string GetAddressComponent(string type)
        {
            if (this.Results == null || this.Results.Length == 0)
                return string.Empty;

            var result = string.Empty;
            type = type.ToLowerInvariant();

            var query = this.Results.Where(googleGeocoding => googleGeocoding.AddressComponents != null && googleGeocoding.AddressComponents.Length != 0)
                .SelectMany(googleGeocoding => googleGeocoding.AddressComponents
                    .Where(addressComponent => addressComponent.Types != null && addressComponent.Types.Length != 0)
                    .Where(addressComponent => addressComponent.Types.Contains(type)));

            foreach (var addressComponent in query)
            {
                result = addressComponent.LongName;
                break; //I only care about the first result
            }

            return result;
        }

        public LatLng Location 
        {
            get
            {
                if (this.Results == null || this.Results.Length == 0)
                    return default(LatLng);

                var query = from geocoding in this.Results
                    where geocoding.Geometry != null
                    select geocoding.Geometry.Location;
                
                return query.FirstOrDefault();
            }
        }
    }
}