using BogdanM.LocationServices.Core;

namespace BogdanM.LocationServices.GoogleMaps.Dtos
{
    public class GoogleMapsGeometry
    {
        public GoogleMapsRectangle Bounds { get; set; }
        public LatLng Location { get; set; }
        public string LocationType { get; set; }
        public GoogleMapsRectangle Viewport { get; set; }
    }
}