using BogdanM.LocationServices.Core;

namespace BogdanM.LocationServices.GoogleMaps.Dtos
{
    public class GoogleMapsLegs
    {
        public GoogleMapsTextValue Distance { get; set; }
        public GoogleMapsTextValue Duration { get; set; }
        public string EndAddress { get; set; }
        public LatLng EndLocation { get; set; }
        public string StartAddress { get; set; }
        public LatLng StartLocation { get; set; }
        public GoogleMapsStep[] Steps { get; set; }
        public string Maneuver { get; set; }
        public string HtmlInstructions { get; set; }
    }
}