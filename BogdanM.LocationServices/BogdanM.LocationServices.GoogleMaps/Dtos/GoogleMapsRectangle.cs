using BogdanM.LocationServices.Core;

namespace BogdanM.LocationServices.GoogleMaps.Dtos
{
    public class GoogleMapsRectangle
    {
        public LatLng Northest { get; set; }
        public LatLng Southwest { get; set; }
    }
}