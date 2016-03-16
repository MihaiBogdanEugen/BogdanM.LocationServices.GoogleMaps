namespace BogdanM.LocationServices.GoogleMaps.Dtos
{
    public class GoogleMapsRoute
    {
        public GoogleMapsGeocodedWaypoint[] GeocodedWaypoints { get; set; }
        public GoogleMapsRoutePart[] Routes { get; set; }
        public string Status { get; set; }
    }
}