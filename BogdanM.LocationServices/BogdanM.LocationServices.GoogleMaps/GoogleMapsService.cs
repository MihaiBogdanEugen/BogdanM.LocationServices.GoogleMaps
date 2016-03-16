using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using BogdanM.LocationServices.Core;
using BogdanM.LocationServices.GoogleMaps.Dtos;
using BogdanM.LocationServices.GoogleMaps.Infrastructure;
using Newtonsoft.Json;

namespace BogdanM.LocationServices.GoogleMaps
{
    /// <summary>
    /// Google Maps API wrapper implementation for basic location services operations like geocoding, reverse geocoding, routing and distance.
    /// Implements IDisposable only for allowing usage with Owin IdentityFactoryOptions creation.
    /// </summary>
    public class GoogleMapsService : BaseLocationService
    {
        private const string DefaultBaseUrl = @"https://maps.google.com/maps/api/geocode/json?address=";
        private const string DefaultRouteUrl = @"https://maps.googleapis.com/maps/api/directions/json?origin={0},{1}&destination={2},{3}&avoid=highways&mode={4}&api_key={5}";

        /// <summary>
        /// Default constructor for the GoogleMaps API Location Service
        /// </summary>
        /// <param name="apiKey">Api Key for allowing usage of the service.</param>
        /// <param name="baseGeocodeUrl">Base format for the url for the geocoding operation.</param>
        /// <param name="baseGeocodeReverseUrl">Base format for the url for the reverse geocoding operation.</param>
        /// <param name="baseRouteUrl">Base format for the url for the routing operation.</param>
        public GoogleMapsService(string apiKey, string baseGeocodeUrl = GoogleMapsService.DefaultBaseUrl, string baseGeocodeReverseUrl = GoogleMapsService.DefaultBaseUrl, string baseRouteUrl = GoogleMapsService.DefaultRouteUrl)
            : base(apiKey, baseGeocodeUrl, baseGeocodeReverseUrl, baseRouteUrl) { }

        /// <summary>
        /// Converts a human-readable address into geographic coordinates.
        /// </summary>
        /// <param name="address">The input address (street name and no, city and country) as an <see cref="Address"/> object.</param>
        /// <returns>The latitude and longitude of the given address as a <see cref="LatLng"/> structure.</returns>
        public override LatLng Geocode(Address address)
        {
            if (address == null)
                throw new ArgumentNullException(nameof(address));

            var url = $"{this.BaseGeocodeUrl}{address}&key={this.ApiKey}";
            url = HttpUtility.UrlPathEncode(url).Replace(" ", "%20");
            var request = WebRequest.CreateHttp(url);
            request.Method = "GET";

            var responseAsString = string.Empty;

            using (var response = request.GetResponse())
            {
                using (var stream = response.GetResponseStream())
                {
                    if (stream != null)
                    {
                        using (var reader = new StreamReader(stream, Encoding.UTF8))
                        {
                            responseAsString = reader.ReadToEnd();
                        }
                    }
                }
            }

            if (string.IsNullOrEmpty(responseAsString))
                return default(LatLng);

            var geocodingResponse = JsonConvert.DeserializeObject<GoogleMapsGeocodingResponse>(responseAsString, new JsonSerializerSettings
            {
                ContractResolver = new SnakeCasePropertyNamesContractResolver()
            });

            return geocodingResponse.Status.ToLowerInvariant() == "ok" ? geocodingResponse.Location : default(LatLng);
        }

        /// <summary>
        /// Async operation for converting a human-readable address into geographic coordinates.
        /// </summary>
        /// <param name="address">The input address (street name and no, city and country) as an <see cref="Address"/> object.</param>
        /// <returns>The latitude and longitude of the given address as a <see cref="Task" /> object.</returns>
        public override async Task<LatLng> GeocodeAsync(Address address)
        {
            if (address == null)
                throw new ArgumentNullException(nameof(address));

            var url = $"{this.BaseGeocodeUrl}{address}&key={this.ApiKey}";
            url = HttpUtility.UrlPathEncode(url).Replace(" ", "%20");
            var request = WebRequest.CreateHttp(url);
            request.Method = "GET";

            var responseAsString = string.Empty;

            using (var response = await request.GetResponseAsync())
            {
                using (var stream = response.GetResponseStream())
                {
                    if (stream != null)
                    {
                        using (var reader = new StreamReader(stream, Encoding.UTF8))
                        {
                            responseAsString = await reader.ReadToEndAsync();
                        }
                    }
                }
            }

            if (string.IsNullOrEmpty(responseAsString))
                return default(LatLng);

            var geocodingResponse = JsonConvert.DeserializeObject<GoogleMapsGeocodingResponse>(responseAsString, new JsonSerializerSettings
            {
                ContractResolver = new SnakeCasePropertyNamesContractResolver()
            });

            return geocodingResponse.Status.ToLowerInvariant() == "ok" ? geocodingResponse.Location : default(LatLng);
        }

        /// <summary>
        /// Converts geographic coordinates into a human-readable address.
        /// </summary>
        /// <param name="point">The input latitude and longitude as a <see cref="LatLng"/> structure.</param>
        /// <returns>The address (street name and no, city and country) as an <see cref="Address"/> object.</returns>
        public override Address ReverseGeocode(LatLng point)
        {
            var latAsString = point.Lat.ToString("###.###############");
            var lngAsString = point.Lng.ToString("###.###############");

            var url = $"{this.BaseGeocodeReverseUrl}{latAsString},{lngAsString}&key={this.ApiKey}";
            url = HttpUtility.UrlPathEncode(url).Replace(" ", "%20");
            var request = WebRequest.CreateHttp(url);
            request.Method = "GET";

            var responseAsString = string.Empty;

            using (var response = request.GetResponse())
            {
                using (var stream = response.GetResponseStream())
                {
                    if (stream != null)
                    {
                        using (var reader = new StreamReader(stream, Encoding.UTF8))
                        {
                            responseAsString = reader.ReadToEnd();
                        }
                    }
                }
            }

            if (string.IsNullOrEmpty(responseAsString))
                return null;

            var geocodingResponse = JsonConvert.DeserializeObject<GoogleMapsGeocodingResponse>(responseAsString, new JsonSerializerSettings
            {
                ContractResolver = new SnakeCasePropertyNamesContractResolver()
            });

            if (geocodingResponse.Status.ToLowerInvariant() != "ok")
                return null;

            return new Address
            {
                StreetName = geocodingResponse.StreetName,
                StreetNo = geocodingResponse.StreetNo
            };
        }

        /// <summary>
        /// Async operation for converting geographic coordinates into a human-readable address.
        /// </summary>
        /// <param name="point">The input latitude and longitude as a <see cref="LatLng"/> structure.</param>
        /// <returns>The address (street name and no, city and country) as a <see cref="Task" /> object.</returns>
        public override async Task<Address> ReverseGeocodeAsync(LatLng point)
        {
            var latAsString = point.Lat.ToString("###.###############");
            var lngAsString = point.Lng.ToString("###.###############");

            var url = $"{this.BaseGeocodeReverseUrl}{latAsString},{lngAsString}&key={this.ApiKey}";
            url = HttpUtility.UrlPathEncode(url).Replace(" ", "%20");
            var request = WebRequest.CreateHttp(url);
            request.Method = "GET";

            var responseAsString = string.Empty;

            using (var response = await request.GetResponseAsync())
            {
                using (var stream = response.GetResponseStream())
                {
                    if (stream != null)
                    {
                        using (var reader = new StreamReader(stream, Encoding.UTF8))
                        {
                            responseAsString = await reader.ReadToEndAsync();
                        }
                    }
                }
            }

            if (string.IsNullOrEmpty(responseAsString))
                return null;

            var geocodingResponse = JsonConvert.DeserializeObject<GoogleMapsGeocodingResponse>(responseAsString, new JsonSerializerSettings
            {
                ContractResolver = new SnakeCasePropertyNamesContractResolver()
            });

            if (geocodingResponse.Status.ToLowerInvariant() != "ok")
                return null;

            return new Address
            {
                StreetName = geocodingResponse.StreetName,
                StreetNo = geocodingResponse.StreetNo
            };
        }

        /// <summary>
        /// Gets the distance in meters between two geographic points.
        /// </summary>
        /// <param name="from">The first geographic point as a <see cref="LatLng"/> structure.</param>
        /// <param name="to">The second geographic point as a <see cref="LatLng"/> structure.</param>
        /// <returns>The distance in meters as an integer.</returns>
        public override int GetDistance(LatLng @from, LatLng to)
        {
            return this.GetGoogleMapsDistanceInMeters(@from, to);
        }

        /// <summary>
        /// Async operation for getting the distance in meters between two geographic points.
        /// </summary>
        /// <param name="from">The first geographic point as a <see cref="LatLng"/> structure.</param>
        /// <param name="to">The second geographic point as a <see cref="LatLng"/> structure.</param>
        /// <returns>The distance in meters as an <see cref="Task"/> object.</returns>
        public override async Task<int> GetDistanceAsync(LatLng @from, LatLng to)
        {
            return await this.GetGoogleMapsDistanceInMetersAsync(@from, to);
        }

        /// <summary>
        /// Returns the route between two geographic points.
        /// </summary>
        /// <param name="from">The first geographic point as a <see cref="LatLng"/> structure.</param>
        /// <param name="to">The second geographic point as a <see cref="LatLng"/> structure.</param>
        /// <returns>An ordered array of <see cref="LatLng"/> structures represeting the route.</returns>
        public override LatLng[] GetRoute(LatLng @from, LatLng to)
        {
            var fromLatAsString = @from.Lat.ToString("###.###############");
            var fromLngAsString = @from.Lng.ToString("###.###############");

            var toLatAsString = to.Lat.ToString("###.###############");
            var toLngAsString = to.Lng.ToString("###.###############");

            var url = string.Format(CultureInfo.InvariantCulture, this.BaseRouteUrl,
                fromLatAsString, fromLngAsString,
                toLatAsString, toLngAsString,
                this.ApiKey);

            url = HttpUtility.UrlPathEncode(url).Replace(" ", "%20");
            var request = WebRequest.CreateHttp(url);
            request.Method = "GET";

            var responseAsString = string.Empty;

            using (var response = request.GetResponse())
            {
                using (var stream = response.GetResponseStream())
                {
                    if (stream != null)
                    {
                        using (var reader = new StreamReader(stream, Encoding.UTF8))
                        {
                            responseAsString = reader.ReadToEnd();
                        }
                    }
                }
            }

            if (string.IsNullOrEmpty(responseAsString))
                return new LatLng[0];

            var route = JsonConvert.DeserializeObject<GoogleMapsRoute>(responseAsString, new JsonSerializerSettings
            {
                ContractResolver = new SnakeCasePropertyNamesContractResolver()
            });

            if (route == null || route.Status.ToLowerInvariant() != "ok" || route.Routes.Length == 0 || route.Routes[0].Legs == null || route.Routes[0].Legs.Length == 0)
                return new LatLng[0];

            return route.Routes[0].Legs[0].Steps.Select(x => new LatLng(x.EndLocation.Lat, x.EndLocation.Lng)).ToArray();
        }

        /// <summary>
        /// Async operation for getting the route between two geographic points.
        /// </summary>
        /// <param name="from">The first geographic point as a <see cref="LatLng"/> structure.</param>
        /// <param name="to">The second geographic point as a <see cref="LatLng"/> structure.</param>
        /// <returns>An ordered array of <see cref="LatLng"/> structures represeting the route in the result of a <see cref="Task"/> object.</returns>
        public override async Task<LatLng[]> GetRouteAsync(LatLng @from, LatLng to)
        {
            var fromLatAsString = @from.Lat.ToString("###.###############");
            var fromLngAsString = @from.Lng.ToString("###.###############");

            var toLatAsString = to.Lat.ToString("###.###############");
            var toLngAsString = to.Lng.ToString("###.###############");

            var url = string.Format(CultureInfo.InvariantCulture, this.BaseRouteUrl,
                fromLatAsString, fromLngAsString,
                toLatAsString, toLngAsString,
                this.ApiKey);

            url = HttpUtility.UrlPathEncode(url).Replace(" ", "%20");
            var request = WebRequest.CreateHttp(url);
            request.Method = "GET";

            var responseAsString = string.Empty;

            using (var response = await request.GetResponseAsync())
            {
                using (var stream = response.GetResponseStream())
                {
                    if (stream != null)
                    {
                        using (var reader = new StreamReader(stream, Encoding.UTF8))
                        {
                            responseAsString = await reader.ReadToEndAsync();
                        }
                    }
                }
            }

            if (string.IsNullOrEmpty(responseAsString))
                return new LatLng[0];

            var route = JsonConvert.DeserializeObject<GoogleMapsRoute>(responseAsString, new JsonSerializerSettings
            {
                ContractResolver = new SnakeCasePropertyNamesContractResolver()
            });

            if (route == null || route.Status.ToLowerInvariant() != "ok" || route.Routes.Length == 0 || route.Routes[0].Legs == null || route.Routes[0].Legs.Length == 0)
                return new LatLng[0];

            return route.Routes[0].Legs[0].Steps.Select(x => new LatLng(x.EndLocation.Lat, x.EndLocation.Lng)).ToArray();
        }

        private int GetGoogleMapsDistanceInMeters(LatLng @from, LatLng to, GoogleMapsTravelMode mode = GoogleMapsTravelMode.Driving)
        {
            var fromLatAsString = @from.Lat.ToString("###.###############");
            var fromLngAsString = @from.Lng.ToString("###.###############");

            var toLatAsString = to.Lat.ToString("###.###############");
            var toLngAsString = to.Lng.ToString("###.###############");

            var url = string.Format(CultureInfo.InvariantCulture, this.BaseRouteUrl,
                fromLatAsString, fromLngAsString,
                toLatAsString, toLngAsString,
                mode.ToString().ToLowerInvariant(),
                this.ApiKey);

            url = HttpUtility.UrlPathEncode(url).Replace(" ", "%20");
            var request = WebRequest.CreateHttp(url);
            request.Method = "GET";

            var responseAsString = string.Empty;

            using (var response = request.GetResponse())
            {
                using (var stream = response.GetResponseStream())
                {
                    if (stream != null)
                    {
                        using (var reader = new StreamReader(stream, Encoding.UTF8))
                        {
                            responseAsString = reader.ReadToEnd();
                        }
                    }
                }
            }

            if (string.IsNullOrEmpty(responseAsString))
                return 0;

            var route = JsonConvert.DeserializeObject<GoogleMapsRoute>(responseAsString, new JsonSerializerSettings
            {
                ContractResolver = new SnakeCasePropertyNamesContractResolver()
            });

            if (route == null || route.Status.ToLowerInvariant() != "ok" || route.Routes.Length == 0 || route.Routes[0].Legs == null || route.Routes[0].Legs.Length == 0)
                return 0;

            return route.Routes[0].Legs[0].Distance.Value;
        }

        private async Task<int> GetGoogleMapsDistanceInMetersAsync(LatLng @from, LatLng to, GoogleMapsTravelMode mode = GoogleMapsTravelMode.Driving)
        {
            var fromLatAsString = @from.Lat.ToString("###.###############");
            var fromLngAsString = @from.Lng.ToString("###.###############");

            var toLatAsString = to.Lat.ToString("###.###############");
            var toLngAsString = to.Lng.ToString("###.###############");

            var url = string.Format(CultureInfo.InvariantCulture, this.BaseRouteUrl,
                fromLatAsString, fromLngAsString,
                toLatAsString, toLngAsString,
                mode.ToString().ToLowerInvariant(),
                this.ApiKey);

            url = HttpUtility.UrlPathEncode(url).Replace(" ", "%20");
            var request = WebRequest.CreateHttp(url);
            request.Method = "GET";

            var responseAsString = string.Empty;

            using (var response = await request.GetResponseAsync())
            {
                using (var stream = response.GetResponseStream())
                {
                    if (stream != null)
                    {
                        using (var reader = new StreamReader(stream, Encoding.UTF8))
                        {
                            responseAsString = await reader.ReadToEndAsync();
                        }
                    }
                }
            }

            if (string.IsNullOrEmpty(responseAsString))
                return 0;

            var route = JsonConvert.DeserializeObject<GoogleMapsRoute>(responseAsString, new JsonSerializerSettings
            {
                ContractResolver = new SnakeCasePropertyNamesContractResolver()
            });

            if (route == null || route.Status.ToLowerInvariant() != "ok" || route.Routes.Length == 0 || route.Routes[0].Legs == null || route.Routes[0].Legs.Length == 0)
                return 0;

            var distance = route.Routes[0].Legs[0].Distance;
            if (string.IsNullOrEmpty(distance?.Text))
                return 0;

            var distanceAsText = distance.Text;
            var parts = distanceAsText.Split(new char[' '], StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 2)
                return 0;

            double miles;
            return double.TryParse(distanceAsText, NumberStyles.AllowDecimalPoint, NumberFormatInfo.InvariantInfo, out miles) ? (int)Math.Ceiling(1609.34 * miles) : 0;
        }
    }
}