using System;

namespace Task1;

public class FileInfo
{
    public FileInfo(string name, long length, string fullName, DateTime lastWriteTime)
    {
        Name = name;
        Length = length;
        FullName = fullName;
        LastWriteTime = lastWriteTime;
    }

    public string Name { get; set; }
    public long Length { get; set; }
    public string FullName { get; set; }
    public DateTime LastWriteTime { get; set; }
}