namespace Dierentuin.Enum
{
    [Flags]
    //I have choosen this flag so i can use binaire numbers. This is so i can easily combine the habitats.
    //For example forest and aqua is 0001 and 0010 to a singular number would be 0011. //
    public enum HabitatType
    {
        None = 0,             // Geen habitat
        Forest = 1,          // 0001
        Aquatic = 2,         // 0010
        Desert = 4,          // 0100
        Grassland = 8        // 1000
    }
}