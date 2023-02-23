using System.IO;

namespace Task1;

public class DriveInfo
{
    public DriveInfo(string name, DriveType type, long allSize, double usagePercent, long freeSpace)
    {
        Name = name;
        Type = type;
        AllSize = allSize;
        UsagePercent = usagePercent;
        FreeSpace = freeSpace;
    }

    public string Name { get; set; }
    public DriveType Type { get; set; }
    public long AllSize { get; set; }
    public double UsagePercent { get; set; }
    public long FreeSpace { get; set; }
}