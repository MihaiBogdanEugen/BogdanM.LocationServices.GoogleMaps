using BogdanM.LocationServices.Core;

namespace BogdanM.LocationServices.GoogleMaps.Dtos
{
    public class GoogleMapsStep
    {
        public GoogleMapsTextValue Distance { get; set; }
        public GoogleMapsTextValue Duration { get; set; }
        public LatLng EndLocation { get; set; }
        public LatLng StartLocation { get; set; }
        public GoogleMapsPolyline Polyline { get; set; }
        public string TravelMode { get; set; }
    }
}