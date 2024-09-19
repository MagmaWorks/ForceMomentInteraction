using OasysUnits;

namespace MagmaWorks.ForceMomentInteraction
{
    public class DiagramSettings
    {
        public Area ConcreteMaximumFaceArea { get; set; } = Area.FromSquareMillimeters(250);
        public Angle ConcreteMinimumAngle { get; set; } = Angle.FromDegrees(25);
        public Area RebarMaximumFaceArea { get; set; } = Area.FromSquareMillimeters(200);
        public Angle RebarMinimumAngle { get; set; } = Angle.FromDegrees(25);
        public int RebarDivisions { get; set; } = 16;
        public int Steps { get; set; } = 30;

        public DiagramSettings() { }

        public DiagramSettings(int steps)
        {
            Steps = steps;
        }

        public DiagramSettings(Area maxArea, Angle minAngle, int steps)
        {
            ConcreteMaximumFaceArea = maxArea;
            ConcreteMinimumAngle = minAngle;
            RebarMaximumFaceArea = maxArea;
            RebarMinimumAngle = minAngle;
            Steps = steps;
        }

        public DiagramSettings(Area maxConcreteArea, Angle minConcreteAngle, Area maxRebarArea, Angle minRebarAngle, int rebarDivisions, int steps)
        {
            ConcreteMaximumFaceArea = maxConcreteArea;
            ConcreteMinimumAngle = minConcreteAngle;
            RebarMaximumFaceArea = maxRebarArea;
            RebarMinimumAngle = minRebarAngle;
            RebarDivisions = rebarDivisions;
            Steps = steps;
        }
    }
}
