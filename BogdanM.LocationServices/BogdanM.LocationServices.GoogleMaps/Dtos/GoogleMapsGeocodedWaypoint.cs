namespace BogdanM.LocationServices.GoogleMaps.Dtos
{
    public class GoogleMapsGeocodedWaypoint
    {
        public string GeocoderStatus { get; set; }
        public string PlaceId { get; set; }
        public string[] Types { get; set; }
    }
}