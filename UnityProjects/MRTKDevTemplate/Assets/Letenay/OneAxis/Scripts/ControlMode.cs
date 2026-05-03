namespace Letenay
{
    public enum ControlMode
    {
        Auto, // controls only rotation, direction selected before movement, reacts to button hover
        Manual, // controls both direction and rotation, reacts to button hover
        Dwell // controls both direction and rotation but with 1s dwell activation
    }
}
