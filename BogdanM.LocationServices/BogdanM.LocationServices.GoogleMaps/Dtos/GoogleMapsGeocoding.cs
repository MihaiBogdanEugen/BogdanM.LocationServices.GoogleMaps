namespace BogdanM.LocationServices.GoogleMaps.Dtos
{
    public class GoogleMapsGeocoding
    {
        public GoogleMapsAddressComponent[] AddressComponents { get; set; }
        public string FormattedAddress { get; set; }
        public GoogleMapsGeometry Geometry { get; set; }
        public bool PartialMatch { get; set; }
        public string PlaceId { get; set; }
        public string[] Types { get; set; }
    }
}